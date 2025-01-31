using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// player evade end state
public class PlayerEvadeEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        #region front or back evade
        switch (playerModel.currentState)
        {
            case PlayerState.Evade_Front_End:
                playerController.PlayAnimation("Evade_Front_End",0.1f);
                break;
            case PlayerState.Evade_Back_End:
                playerController.PlayAnimation("Evade_Back_End", 0.1f);
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

        #region detect fightBackTime
        if (playerModel.fightBack == true && statePlayTime > 0.5f)
        {
            playerModel.fightBack = false;
        }
        #endregion

        #region detect attack
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            if (playerModel.fightBack == true)
            {
                //fight back state
                if (playerModel.currentState == PlayerState.Evade_Front_End)
                    playerController.SwitchState(PlayerState.AttackRush);
                else
                    playerController.SwitchState(PlayerState.AttackRushBack);
                playerModel.fightBack = false;
            }
            else
            {
                //attack state
                playerController.SwitchState(PlayerState.NormalAttack);
            }
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
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            //evade state
            if (playerController.inputMoveVec2.y > 0)
            {
                playerController.SwitchState(PlayerState.Evade_Front);
            }
            else
            {
                playerController.SwitchState(PlayerState.Evade_Back);
            }
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            return;
        }
        #endregion

        #region detect move
        if (playerController.inputMoveVec2 != Vector2.zero)
        {
            switch (playerModel.currentState)
            {
                case PlayerState.Evade_Front_End:
                    playerController.SwitchState(PlayerState.Run);
                    break;
                case PlayerState.Evade_Back_End:
                    playerController.SwitchState(PlayerState.Walk);
                    break;
            }
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
