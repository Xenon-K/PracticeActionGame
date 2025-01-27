using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkState : EnemyStateBase
{
    /// <summary>
    public override void Enter()
    {
        base.Enter();

        enemyController.PlayAnimation("Walk", 0.25f);
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

        #region detect health
        if (enemyController.enemyStats.currentHealth <= 0)
        {
            // Transition to the Death state
            enemyController.SwitchState(EnemyState.Death);
            return;
        }
        #endregion


        #region Movement Logic
        // Get the target's position
        Transform target = enemyController.Target;

        if (target != null)
        {
            float distance = Vector3.Distance(enemyController.transform.position, target.position);

            if (distance <= enemyController.lookRadius && distance > enemyController.Agent.stoppingDistance)
            {
                // Move towards the target
                enemyController.Agent.SetDestination(target.position);
            }
            else if ((distance <= enemyController.lookRadius && distance <= enemyController.Agent.stoppingDistance) || enemyController.Agent.velocity.sqrMagnitude <= 0.01f)
            {
                // Close enough to stop moving

                // Check if the enemy is facing the target
                if (IsFacingTarget(target))
                {
                    enemyController.Agent.ResetPath(); // Stop movement
                    // Switch to attack state when reaching the target and facing them
                    enemyController.SwitchState(EnemyState.NormalAttack);
                    return;
                }
                else
                {
                    // Rotate to face the target
                    FaceTarget(target);
                }
            }
            else if(distance > enemyController.lookRadius)
            {
                //too far
                enemyController.Agent.ResetPath(); // Stop movement
                enemyController.SwitchState(EnemyState.Idle); // Switch to idle state when reaching the target
            }
        }
        #endregion
    }

    /// <summary>
    /// Called when exiting the walk state
    public override void Exit()
    {
        base.Exit();

        enemyController.Agent.ResetPath(); // Stop movement
    }

    /// <summary>
    /// Check if the enemy is facing the target within a certain angle
    private bool IsFacingTarget(Transform target)
    {
        Vector3 directionToTarget = (target.position - enemyController.transform.position).normalized;
        float dotProduct = Vector3.Dot(enemyController.transform.forward, directionToTarget);

        // Check if the target is within a 90-degree cone in front of the enemy
        return dotProduct > 0.5f; // Adjust the threshold as needed (e.g., 0.7f for a narrower angle)
    }

    private void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - enemyController.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Ignore vertical axis
        enemyController.transform.rotation = Quaternion.Slerp(
            enemyController.transform.rotation,
            lookRotation,
            Time.deltaTime * 0.7f // Adjust rotation speed as needed
        );
    }
}
