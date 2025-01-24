using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyStateBase
{
    /// <summary>
    public override void Enter()
    {
        base.Enter();

         enemyController.PlayAnimation("Idle", 0.25f);
    }

    public override void Update()
    {
        base.Update();

        #region detect movement
        Transform target = enemyController.Target;

        if (target != null)
        {
            float distance = Vector3.Distance(enemyController.transform.position, target.position);

            // Check if the player is within the lookRadius but outside the stopping distance
            if (distance <= enemyController.lookRadius && distance > enemyController.Agent.stoppingDistance)
            {
                // Switch to Walk state
                enemyController.SwitchState(EnemyState.Walk);
                return;
            }
            else if (distance <= enemyController.lookRadius && distance <= enemyController.Agent.stoppingDistance)
            {
                enemyController.SwitchState(EnemyState.NormalAttack); // Switch to attck state when reaching the target
            }
        }
        #endregion

        #region detect stun
        if (enemyController.enemyStats.currentResist <= 0)
        {
            // Transition to the Stun state
            enemyController.SwitchState(EnemyState.Stun_Start);
            return;
        }
        #endregion

        #region detect health
        if (enemyController.enemyStats.currentHealth <= 0)
        {
            // Transition to the Death state
            enemyController.SwitchState(EnemyState.Death);
            return;
        }
        #endregion

        if (IsAnimationEnd())
        {
            //back to idle
            enemyController.SwitchState(EnemyState.Idle);
            return;
        }

    }
}
