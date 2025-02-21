using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNormalAttackState : EnemyStateBase
{
    private bool isAttacking = false;
    private bool hitOnce = false;

    /// <summary>
    /// Called when the state is entered.
    public override void Enter()
    {
        base.Enter();

        enemyController.Agent.ResetPath(); // Stop movement

        enemyController.PlayAnimation("Normal_Attack", 0.25f);

        // Enable attack collider at the start of the attack
        enemyController.EnableAttackCollider();
    }

    public override void Update()
    {
        base.Update();

        #region detect stun
        if (enemyController.enemyStats.currentResist <= 0)
        {
            // Transition to the Death state
            enemyController.SwitchState(EnemyState.Stun_Start);
            return;
        }
        #endregion

        #region detect death
        if (enemyController.enemyStats.currentHealth <= 0)
        {
            // Transition to the Death state
            enemyController.SwitchState(EnemyState.Death);
            return;
        }
        #endregion

        #region detect actual attack range
        if (statePlayTime > 1.3f && statePlayTime < 2f)
        {
            isAttacking = true;
        }

        if (statePlayTime >= 2f)
        {
            isAttacking = false;
        }
        #endregion

        // Check if the attack animation has ended
        if (IsAnimationEnd())
        {
            // Back to idle
            enemyController.DisableAttackCollider();
            hitOnce = false;
            isAttacking = false;
            enemyController.SwitchState(EnemyState.Idle);
            return;
        }
    }

    /// <summary>
    /// Handle collision detection for dealing damage.
    public void OnAttackHit(Collider other)
    {
        //Debug.Log($"OnAttackHit triggered with {other.name} tagged as {other.tag}");
        if (isAttacking && !hitOnce && other.CompareTag("Player"))// Detect if the enemy hit the player, and we only hit once
        {
            // Detect if the enemy hit the player
            var playerModelState = other.GetComponent<PlayerModel>();
            var playerStats = other.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                if(playerModelState != null)
                {
                    // Check if the player is evading
                    if (playerModelState.currentState == PlayerState.Evade_Front || playerModelState.currentState == PlayerState.Evade_Back)
                    {
                        playerModelState.fightBack = true;
                        // Trigger special effect for evade
                        TriggerEvadeEffect(playerModelState);
                    }
                    else if (playerModelState.currentState == PlayerState.BigSkillStart || playerModelState.currentState == PlayerState.BigSkill || playerModelState.currentState == PlayerState.SwitchInAttack || playerModelState.currentState == PlayerState.SwitchInAttackEx)
                    {
                        ;//can not get hit during ult or followUp attack
                    }
                    else
                    {
                        // If not evading, apply damage
                        playerStats.TakeDamage(enemyController.enemyStats.damage.GetValue());
                        playerStats.TakeResistDamage(enemyController.enemyStats.resist_damage.GetValue());
                    }
                    //playerStats.TakeDamage(enemyController.enemyStats.damage.GetValue());
                    hitOnce = true;
                    //Debug.Log("Enemy hit the player!");
                }
                
            }
            // Prevent multiple damage triggers during the same attack
            isAttacking = false;
            enemyController.DisableAttackCollider();
        }
    }


    /// <summary>
    /// Trigger a special effect when the player is evading.
    /// <param name="playerModelState">The player's model state.</param>
    private void TriggerEvadeEffect(PlayerModel playerModelState)
    {
        //Debug.Log($"Player is evading with state: {playerModelState.currentState}");

        // Start slow motion for the enemy
        enemyController.ApplySlowMotion(3f);
    }

    /// <summary>
    /// Called when the state is exited.
    public override void Exit()
    {
        base.Exit();

        // Ensure the attack collider is disabled when exiting the state
        hitOnce = false;
        enemyController.DisableAttackCollider();
        isAttacking = false;
    }
}
