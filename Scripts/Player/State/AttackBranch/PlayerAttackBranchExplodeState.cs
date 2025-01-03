using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// branch attack before end state
public class PlayerAttackBranchExplodeState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        //play attack animation
        int successPlayed = playerController.PlayAnimation($"Attack_Branch_{playerModel.skiilConfig.currentNormalAttackIndex}_Explode", 0.1f);
        if (successPlayed < 0) //not found is -1
        {
            playerController.PlayAnimation("Attack_Branch_0_Explode", 0.1f);
        }
        else
        {
            playerModel.skiilConfig.isPerfect = true;
        }
    }

    public override void Update()
    {
        base.Update();

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

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_End);
            return;
        }
        #endregion
    }
}
