using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunEndState : EnemyStateBase
{
    /// <summary>
    public override void Enter()
    {
        enemyController.restoreExChance();
        base.Enter();

        enemyController.PlayAnimation("Stun_End", 0.25f);
        enemyController.enemyStats.RestoreResist();
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

        if (IsAnimationEnd("Stun_End")) //normal EndAnimation does not working properly on this one, since loop is managed by statePlayTime,so IsAnimationEnd can be incorrectly applied to the following state
        {
            //back to normal
            enemyController.SwitchState(EnemyState.Idle);
            return;
        }

    }
}
