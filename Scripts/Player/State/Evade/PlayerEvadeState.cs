using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// player evade state
public class PlayerEvadeState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        #region front or back evade
        switch (playerModel.currentState)
        {
            case PlayerState.Evade_Front:
                playerController.PlayAnimation("Evade_Front", 0.1f);
                break;
            case PlayerState.Evade_Back:
                playerController.PlayAnimation("Evade_Back", 0.1f);
                break;
        }
        #endregion
    }
    public override void Update()
    {
        base.Update();

        #region detect ult state
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.CheckUlt())
        {
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            switch (playerModel.currentState)
            {
                case PlayerState.Evade_Front:
                    playerController.SwitchState(PlayerState.Evade_Front_End);
                    break;
                case PlayerState.Evade_Back:
                    playerController.SwitchState(PlayerState.Evade_Back_End);
                    break;
            }
            return;
        }
        #endregion
    }
}
