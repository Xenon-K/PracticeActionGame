using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRushState : PlayerStateBase
{
    private bool isAttacking = false;
    private bool hitOnce = false;
    public override void Enter()
    {
        base.Enter();

        #region front or back
        switch (playerModel.currentState)
        {
            case PlayerState.AttackRush:
                playerController.PlayAnimation("Attack_Rush", 0.1f);
                break;
            case PlayerState.AttackRushBack:
                playerController.PlayAnimation("Attack_Rush_Back", 0.1f);
                break;
        }
        #endregion

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            //Debug.Log("See enemy: AttackRush");
            playerController.RotateTowards(closestEnemy);
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

        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
            return;
        }
        #endregion

        #region detect evade
        if (playerController.inputSystem.Player.Evade.triggered)
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            //evade state
            if (playerController.inputMoveVec2.y > 0)
            {
                playerController.SwitchState(PlayerState.Evade_Front);
            }
            else
            {
                playerController.SwitchState(PlayerState.Evade_Back);
            }
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            return;
        }
        #endregion

        #region detect ult state
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.CheckUlt())
        {
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //fight back state
            if (playerModel.currentState == PlayerState.AttackRush)
                playerController.SwitchState(PlayerState.AttackRushEnd);
            else
                playerController.SwitchState(PlayerState.AttackRushBackEnd);
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
            Debug.Log("Feel Enemy");
            // Detect if the enemy hit the player
            var enemyStats = other.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(playerController.playerStats.damage.GetValue());
                enemyStats.TakeResistDamage(playerController.playerStats.resist_damage.GetValue());
                playerController.playerStats.GainEnergy(2);
                playerController.ChargeUlt(500);
                hitOnce = true;
                Debug.Log("Player hit the enemy!");
            }

            // Prevent multiple damage triggers during the same attack
            isAttacking = false;
            playerController.ApplyHitLag(0.08f);// hit lag
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
