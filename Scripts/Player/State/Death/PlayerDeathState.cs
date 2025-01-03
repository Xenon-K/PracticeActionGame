using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerController.PlayAnimation("Death", 0.25f);
    }

    public override void Update()
    {
        base.Update();

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            playerController.SwitchNextModel();
            playerController.controllableCounter--;
            return;
        }
        #endregion

    }
}
