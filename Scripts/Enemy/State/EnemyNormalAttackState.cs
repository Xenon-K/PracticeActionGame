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

        enemyController.PlayAnimation("Normal_Attack", 0.25f);

        // Enable attack collider at the start of the attack
        enemyController.EnableAttackCollider();
    }

    public override void Update()
    {
        base.Update();

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
        if (isAttacking && !hitOnce && other.CompareTag("Player"))
        {
            // Detect if the enemy hit the player
            var playerStats = other.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(enemyController.enemyStats.damage.GetValue());
                hitOnce = true;
                Debug.Log("Enemy hit the player!");
            }

            // Prevent multiple damage triggers during the same attack
            isAttacking = false;
            enemyController.DisableAttackCollider();
        }
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
