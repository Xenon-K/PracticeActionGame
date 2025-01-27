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
        playerController.UsedUlt();
        base.Enter();
        playerController.shotPanel.StartAndEndScene();
        //switch camera
        CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
        CameraManager.INSTANCE.freeLookCanmera.SetActive(false);
        playerModel.bigSkillStartShot.SetActive(true);

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            Debug.Log("See enemy: Ult");
            playerController.RotateTowards(closestEnemy);
        }

        //attack afterswing animation
        playerController.PlayAnimation("BigSkill_Start", 0.0f);
    }

    public override void Update()
    {
        base.Update();

        Transform closestEnemy = playerController.FindClosestEnemy(10f);
        if (closestEnemy != null)
        {
            playerController.RotateTowards(closestEnemy);
        }

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
