using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// walking branch attack state
public class PlayerAttackBranchWalkState : PlayerStateBase
{
    private bool isAttacking = false;
    private bool hasEnergy = false;
    public override void Enter()
    {
        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
            return;
        }
        #endregion

        base.Enter();
        //check if we can continue
        hasEnergy = playerController.playerStats.useConstantSkill();
        if (!hasEnergy)
        {
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_Explode);
            return;
        }
        //reset input buffer for continuous attack
        playerModel.skiilConfig.HoldingComboed = false;

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            Debug.Log("See enemy: Walk");
            playerController.RotateTowards(closestEnemy);
        }

        //play attack animation
        int successPlayed = playerController.PlayAnimation($"Attack_Branch_{playerModel.skiilConfig.currentNormalAttackIndex}_Walk", 0.1f);
        if (successPlayed < 0) //not found is -1
        {
            playerController.PlayAnimation("Attack_Branch_0_Walk", 0.1f);
        }
        else
        {
            playerModel.skiilConfig.isPerfect = true;
        }
        // Enable attack collider at the start of the attack
        playerController.EnableAttackCollider();
    }

    public override void Update()
    {
        base.Update();

        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
            return;
        }
        #endregion

        #region detect actual attack range
        if (statePlayTime > 0.1f && statePlayTime < 1f)
        {
            isAttacking = true;

            Transform closestEnemy = playerController.FindClosestEnemy(10f);
            if (closestEnemy != null)
            {
                playerController.RotateTowards(closestEnemy);
            }
        }

        if (statePlayTime >= 1f)
        {
            // Enable attack collider for next loop attack
            playerController.EnableAttackCollider();
            isAttacking = false;
            ResetTimer();//reset hit timer
        }
        #endregion

        // Exit this state if the branch attack key is no longer held
        if (!playerController.inputSystem.Player.AttackBranch.IsPressed() && !playerController.inputSystem.Player.AttackBranch.triggered || !hasEnergy)
        {
            //input buffer for continuous attack
            playerModel.skiilConfig.HoldingComboed = false;
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_Explode);
            return;
        }

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.CheckUlt())
        {
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region detect walk for loop
        if (playerController.inputSystem.Player.AttackBranch.IsPressed() && playerController.inputMoveVec2.y > 0)
        {
            //switch to walk attack
            playerController.SwitchState(PlayerState.Attack_Branch_Walk);
            //input buffer for continuous attack
            playerModel.skiilConfig.HoldingComboed = true;
            return;
        }
        #endregion

        #region detect attack for loop
        if (playerController.inputSystem.Player.AttackBranch.IsPressed() && (playerController.inputMoveVec2.y < 0 || playerController.inputMoveVec2 == Vector2.zero))
        {
            //more loop attack
            playerController.SwitchState(PlayerState.Attack_Branch_Loop);
            //input buffer for continuous attack
            playerModel.skiilConfig.HoldingComboed = true;
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd() && !playerModel.skiilConfig.HoldingComboed)
        {
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_Explode);
            return;
        }
        #endregion
    }

    /// <summary>
    /// Handle collision detection for dealing damage.
    public void OnAttackHit(Collider other)
    {
        //Debug.Log($"OnAttackHit triggered with {other.name} tagged as {other.tag}");
        //if (!isAttacking)
        //Debug.Log("is not attacking");
        //if (hitOnce)
        //Debug.Log("hit");
        //if (!other.CompareTag("Enemy"))
        //Debug.Log("not tag");
        if (isAttacking && other.CompareTag("Enemy"))
        {
            //Debug.Log("Feel Enemy");
            // Detect if the enemy hit the player
            var enemyStats = other.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(playerController.playerStats.damage.GetValue());
                enemyStats.TakeResistDamage(playerController.playerStats.resist_damage.GetValue());
                hasEnergy = playerController.playerStats.useConstantSkill();
                playerController.ChargeUlt(100);
                Debug.Log("Player hit the enemy!");
            }

            // Prevent multiple damage triggers during the same attack
            isAttacking = false;
            playerController.DisableAttackCollider();
        }
    }

    /// <summary>
    /// Called when the state is exited.
    public override void Exit()
    {
        base.Exit();

        // Ensure the attack collider is disabled when exiting the state
        playerController.DisableAttackCollider();
        isAttacking = false;
    }
}
