using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack branch state
public class PlayerAttackBranchState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        //play attack animation
        int successPlayed = playerController.PlayAnimation("Attack_Branch_" + playerModel.skiilConfig.currentNormalAttackIndex, 0.1f);
        if (successPlayed < 0) //not found is -1
        {
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            playerController.PlayAnimation("Attack_Branch_0", 0.1f);
        }
        else
        {
            playerModel.skiilConfig.isPerfect = true;
        }
    }

    public override void Update()
    {
        base.Update();

        #region detect combo
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            playerModel.skiilConfig.HasComboed = true;
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

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            #region detect attack
            if (playerController.inputSystem.Player.Fire.triggered || playerModel.skiilConfig.HasComboed)
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
            //end branch attack
            playerController.SwitchState(PlayerState.Attack_Branch_Loop);
            return;
        }
        #endregion
    }
}
