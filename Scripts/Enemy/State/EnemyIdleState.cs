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
            if (distance <= enemyController.lookRadius)//simplified version
            {
                // Switch to Walk state
                enemyController.SwitchState(EnemyState.Walk);
                return;
            }
            /*
            // Check if the player is within the lookRadius but outside the stopping distance
            if (distance <= enemyController.lookRadius && distance > enemyController.Agent.stoppingDistance || !IsFacingTarget(target))
            {
                // Switch to Walk state
                enemyController.SwitchState(EnemyState.Walk);
                return;
            }
            else if (distance <= enemyController.lookRadius && distance <= enemyController.Agent.stoppingDistance)
            {
                // Check if the enemy is facing the target
                if (IsFacingTarget(target))
                {
                    // Switch to attack state when reaching the target and facing them
                    enemyController.SwitchState(EnemyState.NormalAttack);
                    return;
                }
            }
            */
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
    /*
    /// <summary>
    /// Check if the enemy is facing the target within a certain angle
    private bool IsFacingTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - enemyController.transform.position).normalized;
        float dotProduct = Vector3.Dot(enemyController.transform.forward, directionToTarget);

        // Check if the target is within a 90-degree cone in front of the enemy
        return dotProduct > 0.5f; // Adjust the threshold as needed (e.g., 0.7f for a narrower angle)
    }
    */
}
