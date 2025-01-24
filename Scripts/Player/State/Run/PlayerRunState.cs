using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// run state
public class PlayerRunState : PlayerStateBase
{
    private Camera mainCamera;

    public override void Enter()
    {
        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
            return;
        }
        #endregion

        base.Enter();

        mainCamera = Camera.main;

        //run state
        switch (playerModel.currentState)
        {
            case PlayerState.Walk:
                #region left or right foot
                switch (playerModel.foot)
                {
                    case ModelFoot.Left:
                        playerController.PlayAnimation("Walk", 0.125f, 0.5f);
                        playerModel.foot = ModelFoot.Right;
                        break;
                    case ModelFoot.Right:
                        playerController.PlayAnimation("Walk", 0.125f, 0.0f);
                        playerModel.foot = ModelFoot.Left;
                        break;
                }
                #endregion
                break;
            case PlayerState.Run:
                #region left or right foot
                switch (playerModel.foot)
                {
                    case ModelFoot.Left:
                        playerController.PlayAnimation("Run", 0.125f, 0.5f);
                        playerModel.foot = ModelFoot.Right;
                        break;
                    case ModelFoot.Right:
                        playerController.PlayAnimation("Run", 0.125f, 0.0f);
                        playerModel.foot = ModelFoot.Left;
                        break;
                }
                #endregion
                break;
        }
    }
    public override void Update()
    {
        base.Update();

        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
            return;
        }
        #endregion

        #region detect run for sure
        if (statePlayTime > 0.5f)
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
        }
        #endregion

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered)
        {
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region detect attack
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            //attack state
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

        #region detect evade
        if (playerController.inputSystem.Player.Evade.triggered)
        {
            //evade state
            playerController.SwitchState(PlayerState.Evade_Front);
            return;
        }
        #endregion

        #region detect idle
        if (playerController.inputMoveVec2 == Vector2.zero)
        {
            playerController.SwitchState(PlayerState.RunEnd);
            return;
        }
        #endregion
        else
        {
            #region direction
            Vector3 inputMoveVec3 = new Vector3(playerController.inputMoveVec2.x, 0, playerController.inputMoveVec2.y);
            //Get the camera's rotation axis Y
            float cameraAxisY = mainCamera.transform.rotation.eulerAngles.y;
            //Quaternion times Vector
            Vector3 targetDic = Quaternion.Euler(0, cameraAxisY, 0) * inputMoveVec3;
            Quaternion targetQua = Quaternion.LookRotation(targetDic);

            //rotate angle
            float angles = Mathf.Abs(targetQua.eulerAngles.y - playerModel.transform.eulerAngles.y);
            if (angles > 145f && playerController.inputMoveVec2.magnitude > 0.9f && angles < 215f && playerModel.currentState == PlayerState.Run)  
            {
                //turn back state
                playerController.SwitchState(PlayerState.TurnBack);
            }
            else
            {
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, targetQua, Time.deltaTime * playerController.rotationSpeed);
            }
            #endregion
        }

        #region jog to run
        if (playerModel.currentState == PlayerState.Walk && statePlayTime > 1.5f) 
        {
            //run state
            playerController.SwitchState(PlayerState.Run);
            return;
        }
        #endregion
    }
}
