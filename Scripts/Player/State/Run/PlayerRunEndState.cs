using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// suddent stop
public class PlayerRunEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        #region left or right foot
        switch (playerModel.foot)
        {
            case ModelFoot.Right:
                playerController.PlayAnimation("Run_End_R",0.1f);
                break;
            case ModelFoot.Left:
                playerController.PlayAnimation("Run_End_L", 0.1f);
                break;
        }
        #endregion
    }

    public override void Update()
    {
        base.Update();

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

        #region detect move
        if (playerController.inputMoveVec2 != Vector2.zero)
        {
            playerController.SwitchState(PlayerState.Walk);
            return;
        }
        #endregion

        #region detect evade
        if (playerController.inputSystem.Player.Evade.IsPressed())
        {
            //evade state
            playerController.SwitchState(PlayerState.Evade_Back);
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd()) 
        {
            playerController.SwitchState(PlayerState.Idle);
            return;
        }
        #endregion
    }
}
