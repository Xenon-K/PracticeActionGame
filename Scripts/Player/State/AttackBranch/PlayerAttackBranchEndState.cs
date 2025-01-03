using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack branch end state
public class PlayerAttackBranchEndState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        //reset input buffer for continuous attack
        playerModel.skiilConfig.HoldingComboed = false;
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            playerModel.skiilConfig.HasComboed = true;
        }
        //attack afterswing animation
        int successPlayed = playerController.PlayAnimation($"Attack_Branch_{playerModel.skiilConfig.currentNormalAttackIndex}_End", 0.0f);
        if (successPlayed < 0) //not found is -1
        {
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;

            playerController.PlayAnimation("Attack_Branch_0_End", 0.1f);
        }
    }

    public override void Update()
    {
        base.Update();

        #region detect attack
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            if (!playerModel.skiilConfig.isPerfect)
            {
                // reset combo
                playerModel.skiilConfig.currentNormalAttackIndex = 1;
            }
            else
            {
                //Accumulate attack combos
                playerModel.skiilConfig.currentNormalAttackIndex++;
            }
            //back to normal attack
            playerController.SwitchState(PlayerState.NormalAttack);
            return;
        }
        #endregion

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered)
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

        #region detect attack branch
        if (playerController.inputSystem.Player.AttackBranch.triggered)
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
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
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
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
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            //switch back to idle
            playerController.SwitchState(PlayerState.Idle);
            return;
        }
        #endregion

        #region detect move
        if (!playerModel.skiilConfig.HasComboed && playerController.inputMoveVec2 != Vector2.zero)
        {
            //switch to movement
            playerController.SwitchState(PlayerState.Walk);
            return;
        }
        #endregion
    }
}
