using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// unl transition state
public class PlayerBigSkillTransitionState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        
        //switch camera
        playerModel.bigSkillShot.SetActive(false);
        CameraManager.INSTANCE.cm_brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1f);
        CameraManager.INSTANCE.freeLookCanmera.SetActive(true);
        CameraManager.INSTANCE.ResetFreeLookCamera();
        
        //afterswing
        playerController.PlayAnimation("BigSkillTransition", 0.0f);
    }

    public override void Update()
    {
        base.Update();

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.CheckUlt())
        {
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
            return;
        }
        #endregion

        #region detect attack
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            //back to normal attack
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion

        #region detect attack branch
        if (playerController.inputSystem.Player.AttackBranch.triggered)
        {
            //attack state
            playerController.SwitchState(PlayerState.Attack_Branch);
            return;
        }
        #endregion

        #region detect move
        if (playerController.inputMoveVec2 != Vector2.zero)
        {
            //switch to movement
            playerController.SwitchState(PlayerState.Walk);
            return;
        }
        #endregion

        #region detect evade
        if (playerController.inputSystem.Player.Evade.triggered)
        {
            //switch to evade
            if (playerController.inputMoveVec2.y > 0)
            {
                playerController.SwitchState(PlayerState.Evade_Front);
            }
            else
            {
                playerController.SwitchState(PlayerState.Evade_Back);
            }
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //end BigSkill
            playerController.SwitchState(PlayerState.BigSkillEnd);
            return;
        }
        #endregion
    }
}
