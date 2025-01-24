using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunState : EnemyStateBase
{
    /// <summary>
    public override void Enter()
    {
        base.Enter();

        enemyController.PlayAnimation("Stun_Loop", 0.25f);
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

        if (statePlayTime >= 5f)
        {
            //stun end
            enemyController.SwitchState(EnemyState.Stun_End);
            return;
        }

    }
}
