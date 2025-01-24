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
                enemyController.Agent.ResetPath(); // Stop movement
                enemyController.SwitchState(EnemyState.NormalAttack); // Switch to attack state when reaching the target
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
}
