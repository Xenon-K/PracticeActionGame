using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBornState : EnemyStateBase
{
    public override void Enter()
    {
        base.Enter();

        enemyController.PlayAnimation("Born", 0f);
    }

    public override void Update()
    {
        base.Update();

        #region detect health
        if (enemyController.enemyStats.currentHealth <= 0)
        {
            // Transition to the Death state
            enemyController.SwitchState(EnemyState.Death);
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //back to idle
            enemyController.SwitchState(EnemyState.Idle);
            return;
        }
        #endregion
    }
}
