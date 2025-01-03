using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyStateBase
{
    /// <summary>
    public override void Enter()
    {
        base.Enter();

        enemyController.PlayAnimation("Death", 0.25f);
        enemyController.Agent.ResetPath(); // Stop movement
    }

    public override void Update()
    {
        base.Update();

        if (IsAnimationEnd())
        {
            //end scene
            GameObject.Destroy(enemyController.gameObject);
            return;
        }

    }
}
