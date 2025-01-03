using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ult state
public class PlayerBigSkillState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        //switch camera
        playerModel.bigSkillStartShot.SetActive(false);
        playerModel.bigSkillShot.SetActive(true);

        //afterswing
        playerController.PlayAnimation("BigSkill", 0.0f);
    }

    public override void Update()
    {
        base.Update();

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //end BigSkill
            playerController.SwitchState(PlayerState.BigSkillTransition);
            return;
        }
        #endregion
    }
}
