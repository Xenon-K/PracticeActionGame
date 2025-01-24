using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle, Idle_AFK,
    Walk, Run, RunEnd, TurnBack,
    Evade_Front, Evade_Back, Evade_Front_End, Evade_Back_End,AttackRush,AttackRushEnd,AttackRushBack, AttackRushBackEnd,
    NormalAttack, NormalAttackEnd,
    BigSkillStart, BigSkill, BigSkillTransition, BigSkillEnd,
    SwitchInNormal, SwitchInAttack, SwitchInAttackEnd, SwitchInAttackEx, SwitchInAttackExEnd,
    Attack_Branch, Attack_Branch_End,Attack_Branch_Loop,Attack_Branch_Walk, Attack_Branch_Explode,
    Death,Hit,
}
public class PlayerStateBase : StateBase
{
    //player controller
    protected PlayerController playerController;
    //player model
    protected PlayerModel playerModel;
    //animation info
    private AnimatorStateInfo stateInfo;
    //Record the time when the current state is entered
    protected float statePlayTime = 0;

    public override void Init(IStateMachineOwner owner)
    {
        playerController = (PlayerController)owner;
        playerModel = playerController.playerModel;
    }

    public override void Enter()
    {
        statePlayTime = 0;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
        //apply gravity
        playerModel.characterController.Move(new Vector3(0, playerModel.gravity * Time.deltaTime, 0));
        //refresh animation status
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);
    }

    public override void LateUpdate()
    {
    }

    public override void UnInit()
    {

    }

    public override void Update()
    {
        //apply gravity
        playerModel.characterController.Move(new Vector3(0, playerModel.gravity * Time.deltaTime, 0));
        //Status entry time
        statePlayTime += Time.deltaTime;

        #region Detecting character switching
        if (playerController.inputSystem.Player.Fire.triggered &&
            playerModel.currentState != PlayerState.BigSkillStart &&
            playerModel.currentState != PlayerState.BigSkill &&
            playerController.canEx == true)
        {
            //character switch
            playerController.SwitchNextExModel();
        }
        else if (playerController.inputSystem.Player.SwitchDown.triggered &&
            playerModel.currentState != PlayerState.BigSkillStart &&
            playerModel.currentState != PlayerState.BigSkill && 
            playerModel.currentState == PlayerState.Hit)
        {
            //character switch
            playerController.SwitchNextSupportModel();
        }
        else if (playerController.inputSystem.Player.SwitchDown.triggered &&
            playerModel.currentState!= PlayerState.BigSkillStart&&
            playerModel.currentState != PlayerState.BigSkill)
        {
            //character switch
            playerController.SwitchNextModel();
        }
        if (playerController.inputSystem.Player.SwitchLast.triggered &&
            playerModel.currentState != PlayerState.BigSkillStart &&
            playerModel.currentState != PlayerState.BigSkill &&
            playerController.canEx == true)
        {
            //character switch
            playerController.SwitchLastExModel();
        }
        else if (playerController.inputSystem.Player.SwitchUp.triggered &&
        playerModel.currentState != PlayerState.BigSkillStart &&
        playerModel.currentState != PlayerState.BigSkill)
        {
            //character switch
            playerController.SwitchLastSupportModel();
        }
        else if (playerController.inputSystem.Player.SwitchUp.triggered &&
        playerModel.currentState != PlayerState.BigSkillStart &&
        playerModel.currentState != PlayerState.BigSkill &&
            playerModel.currentState == PlayerState.Hit)
        {
            //character switch
            playerController.SwitchLastModel();
        }
        #endregion
    }

    /// <summary>
    /// Determine whether the animation is finished
    public bool IsAnimationEnd()
    {
        //refresh animation status
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1.0f && !playerModel.animator.IsInTransition(0);
    }

    /// <summary>
    /// Get animation progress
    /// <returns>animation progress</returns>
    public float NormalizedTime()
    {
        //refresh animator state
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime;
    }

    /// <summary>
    /// Reset stateTimer
    public void ResetTimer()
    {
        statePlayTime = 0;
    }
}
