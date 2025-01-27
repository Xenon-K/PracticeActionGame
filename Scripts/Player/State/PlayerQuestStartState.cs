using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuestStartState : PlayerStateBase
{
    bool enterOnce = false;
    public override void Enter()
    {
        base.Enter();
        //switch camera
        CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0f);
        CameraManager.INSTANCE.freeLookCanmera.SetActive(false);
        playerModel.QuestStartShot.SetActive(true);

        playerController.PlayAnimation("QuestStart", 0f);
    }

    public override void Update()
    {
        base.Update();

        #region call shot panelscene because this function does not work in start awake or enter
        if (!enterOnce) 
        {
            playerController.shotPanel.StartAndEndOpenScene();
            enterOnce = true;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //switch camera
            playerModel.QuestStartShot.SetActive(false);
            CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1f);
            CameraManager.INSTANCE.freeLookCanmera.SetActive(true);
            CameraManager.INSTANCE.ResetFreeLookCamera();
            //back to idle
            playerController.SwitchState(PlayerState.Idle);
            return;
        }
        #endregion
    }
}
