using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loop Attack branch state
public class PlayerAttackBranchLoopState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        //reset input buffer for continuous attack
        playerModel.skiilConfig.HoldingComboed = false;
        //play attack animation
        int successPlayed = playerController.PlayAnimation($"Attack_Branch_{playerModel.skiilConfig.currentNormalAttackIndex}_Loop", 0.1f);
        if (successPlayed < 0) //not found is -1
        {
            playerController.PlayAnimation("Attack_Branch_0_Loop", 0.1f);
        }
        else
        {
            playerModel.skiilConfig.isPerfect = true;
        }
    }

    public override void Update()
    {
        base.Update();

        // Exit this state if the branch attack key is no longer held
        if (!playerController.inputSystem.Player.AttackBranch.IsPressed())
        {
            //input buffer for continuous attack
            playerModel.skiilConfig.HoldingComboed = false;
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_Explode);
            return;
        }

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered)
        {
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region detect walk for loop
        if (playerController.inputSystem.Player.AttackBranch.IsPressed() && playerController.inputMoveVec2.y > 0)
        {
            //switch to walk attack
            playerController.SwitchState(PlayerState.Attack_Branch_Walk);
            //input buffer for continuous attack
            playerModel.skiilConfig.HoldingComboed = true;
            return;
        }
        #endregion

        #region detect attack for loop
        if (playerController.inputSystem.Player.AttackBranch.IsPressed() && (playerController.inputMoveVec2.y < 0 || playerController.inputMoveVec2 == Vector2.zero))
        {
            //more loop attack
            playerController.SwitchState(PlayerState.Attack_Branch_Loop);
            //input buffer for continuous attack
            playerModel.skiilConfig.HoldingComboed = true;
            return;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd() && !playerModel.skiilConfig.HoldingComboed)
        {
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_Explode);
            return;
        }
        #endregion
    }
}
