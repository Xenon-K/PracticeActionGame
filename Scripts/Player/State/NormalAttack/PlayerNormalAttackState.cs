using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attack state
/// </summary>
public class PlayerNormalAttackState : PlayerStateBase
{
    private bool enterNextAttack;

    public override void Enter()
    {
        base.Enter();

        playerModel.skiilConfig.HasComboed = false;

        enterNextAttack = false;
        //play attack animation
        if (playerModel.skiilConfig.isPerfect)
        {
            playerController.PlayAnimation($"Attack_Normal_{playerModel.skiilConfig.currentNormalAttackIndex}_Perfect", 0.1f);
        }
        else
        {
            playerController.PlayAnimation("Attack_Normal_" + playerModel.skiilConfig.currentNormalAttackIndex, 0.1f);
        }
    }

    public override void Update()
    {
        base.Update();

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

        // detect combo
        if (NormalizedTime() >= 0.5f && playerController.inputSystem.Player.Fire.triggered) 
        {
            enterNextAttack = true;
        }

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            if (enterNextAttack)
            {
                // next attack 
                //Accumulate attack combos
                playerModel.skiilConfig.currentNormalAttackIndex++;
                if (playerModel.skiilConfig.currentNormalAttackIndex > playerModel.skiilConfig.normalAttackDamageMultiple.Length)
                {
                    // reset combo
                    playerModel.skiilConfig.currentNormalAttackIndex = 1;
                }

                //normal sttack state
                playerController.SwitchState(PlayerState.NormalAttack);
                return;
            }
            else
            {
                //afterswing for this combo
                playerController.SwitchState(PlayerState.NormalAttackEnd);
                return;
            }
        }
        #endregion
    }
}
