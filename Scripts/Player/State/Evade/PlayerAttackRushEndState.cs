using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRushEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        //play afterswing animation
        #region front or back
        switch (playerModel.currentState)
        {
            case PlayerState.AttackRushEnd:
                playerController.PlayAnimation("Attack_Rush_End", 0.1f);
                break;
            case PlayerState.AttackRushBackEnd:
                playerController.PlayAnimation("Attack_Rush_Back_End", 0.1f);
                break;
        }
        #endregion
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
            //switch back to idle
            playerController.SwitchState(PlayerState.Idle);
            return;
        }
        #endregion
    }
}
