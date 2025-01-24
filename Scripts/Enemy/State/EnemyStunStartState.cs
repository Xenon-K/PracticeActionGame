using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunStartState : EnemyStateBase
{
    /// <summary>
    public override void Enter()
    {
        base.Enter();

        //give ex chances
        enemyController.PlayerExChance();

        enemyController.PlayAnimation("Stun_Start", 0.25f);
        enemyController.Agent.ResetPath(); // Stop movement
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

        if (IsAnimationEnd())
        {
            //enter stun
            enemyController.SwitchState(EnemyState.Stun_Loop);
            return;
        }

    }
}
