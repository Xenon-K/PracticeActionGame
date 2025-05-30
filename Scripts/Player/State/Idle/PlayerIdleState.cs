using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        switch (playerModel.currentState) 
        {
            case PlayerState.Idle:
                playerController.PlayAnimation("Idle", 0.25f);
                break;
            case PlayerState.Idle_AFK:
                playerController.PlayAnimation("Idle_AFK", 0.25f);
                break;
        }  
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

        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
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

        #region detect walk
        if (playerController.inputMoveVec2 != Vector2.zero)
        {
            //walk state
            playerController.SwitchState(PlayerState.Walk);
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

        switch (playerModel.currentState)
        {
            case PlayerState.Idle:
                #region detect afk
                if (statePlayTime > 3) 
                {
                    //afk state
                    playerController.SwitchState(PlayerState.Idle_AFK);
                    return;
                }
                #endregion
                break;
            case PlayerState.Idle_AFK:
                #region detect afk ended
                if (IsAnimationEnd()) 
                {
                    //back to idle
                    playerController.SwitchState(PlayerState.Idle);
                    return;
                }
                #endregion
                break;
        }

    }
}
