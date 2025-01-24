using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack branch state
public class PlayerAttackBranchState : PlayerStateBase
{
    private bool isAttacking = false;
    private bool hitOnce = false;
    public override void Enter()
    {
        base.Enter();
        //play attack animation
        int successPlayed = playerController.PlayAnimation("Attack_Branch_" + playerModel.skiilConfig.currentNormalAttackIndex, 0.1f);

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            //Debug.Log("See enemy: Branch");
            playerController.RotateTowards(closestEnemy);
        }

        if (successPlayed < 0) //not found is -1
        {
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            playerController.PlayAnimation("Attack_Branch_0", 0.1f);
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

        #region detect actual attack range
        if (statePlayTime > 0.1f && statePlayTime < 2f)
        {
            isAttacking = true;

            Transform closestEnemy = playerController.FindClosestEnemy(10f);
            if (closestEnemy != null)
            {
                playerController.RotateTowards(closestEnemy);
            }
        }

        if (statePlayTime >= 2f)
        {
            isAttacking = false;
        }
        #endregion

        #region detect combo
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            playerModel.skiilConfig.HasComboed = true;
        }
        #endregion

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered)
        {
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            #region detect attack
            if (playerController.inputSystem.Player.Fire.triggered || playerModel.skiilConfig.HasComboed)
            {
                if (!playerModel.skiilConfig.isPerfect)
                {
                    // reset combo
                    playerModel.skiilConfig.currentNormalAttackIndex = 1;
                }
                else
                {
                    //Accumulate attack combos
                    playerModel.skiilConfig.currentNormalAttackIndex++;
                }
                //back to normal attack
                playerController.SwitchState(PlayerState.NormalAttack);
                return;
            }
            #endregion
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_Loop);
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
        if (isAttacking && !hitOnce && other.CompareTag("Enemy"))
        {
            //Debug.Log("Feel Enemy");
            // Detect if the enemy hit the player
            var enemyStats = other.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(playerController.playerStats.damage.GetValue());
                enemyStats.TakeResistDamage(playerController.playerStats.resist_damage.GetValue());
                hitOnce = true;
                var enemyController = other.GetComponent<EnemyController>();
                if (enemyController.exChance > 0)
                {
                    enemyController.usedExChance();
                    playerController.canEx = true;
                    playerController.ApplyGlobalSlowMotion(3f);
                    playerController.BroadcastCurrentOrder();
                }
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
        hitOnce = false;
        playerController.DisableAttackCollider();
        isAttacking = false;
    }
}
