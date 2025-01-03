using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// unl start state
public class PlayerBigSkillStartState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        //switch camera
        CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
        CameraManager.INSTANCE.freeLookCanmera.SetActive(false);
        playerModel.bigSkillStartShot.SetActive(true);

       //attack afterswing animation
       playerController.PlayAnimation("BigSkill_Start", 0.0f);
    }

    public override void Update()
    {
        base.Update();

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //switch to actual BigSkill
            playerController.SwitchState(PlayerState.BigSkill);
            return;
        }
        #endregion
    }
}
