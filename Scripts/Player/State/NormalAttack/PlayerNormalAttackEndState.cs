using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// afterswing
public class PlayerNormalAttackEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        //play afterswing animation
        if (playerModel.skiilConfig.isPerfect)
        {
            playerController.PlayAnimation($"Attack_Normal_{playerModel.skiilConfig.currentNormalAttackIndex}_Perfect_End", 0.1f);
        }
        else
        {
            playerController.PlayAnimation($"Attack_Normal_{playerModel.skiilConfig.currentNormalAttackIndex}_End", 0.1f);
        }
    }

    public override void Update()
    {
        base.Update();

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.CheckUlt())
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region detect attack

        if (playerController.inputSystem.Player.Fire.triggered)
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            //attack combo
            playerModel.skiilConfig.currentNormalAttackIndex++;
            if (playerModel.skiilConfig.currentNormalAttackIndex > playerModel.skiilConfig.normalAttackDamageMultiple.Length)
            {
                // reset combo
                playerModel.skiilConfig.currentNormalAttackIndex = 1;
            }

            //switch to normal attack
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion

        #region detect attack branch
        if (playerController.inputSystem.Player.AttackBranch.triggered)
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            //attack state
            playerController.SwitchState(PlayerState.Attack_Branch);
            return;
        }
        #endregion

        #region detect move
        if (playerController.inputMoveVec2 != Vector2.zero && statePlayTime > 0.5f)
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            //walk state
            playerController.SwitchState(PlayerState.Walk);
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
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

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            //back to idle
            playerController.SwitchState(PlayerState.Idle);
            //reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            return;
        }
        #endregion
    }
}
