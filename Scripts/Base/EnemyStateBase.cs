using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Born,
    Idle,
    Walk,
    NormalAttack,
    Death
}
public class EnemyStateBase : StateBase
{
    //enemy controller
    protected EnemyController enemyController;
    //enemy model
    protected EnemyModel enemyModel;
    //animation info
    private AnimatorStateInfo stateInfo;
    //Record the time when the current state is entered
    protected float statePlayTime = 0;

    public override void Init(IStateMachineOwner owner)
    {
        enemyController = (EnemyController)owner;
        enemyModel = enemyController.enemyModel;
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
        //refresh animation status
        stateInfo = enemyModel.animator.GetCurrentAnimatorStateInfo(0);
    }

    public override void LateUpdate()
    {
    }

    public override void UnInit()
    {

    }

    public override void Update()
    {
        //Status entry time
        statePlayTime += Time.deltaTime;
    }

    /// <summary>
    /// Determine whether the animation is finished
    public bool IsAnimationEnd()
    {
        //refresh animation status
        stateInfo = enemyModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1.0f && !enemyModel.animator.IsInTransition(0);
    }

    /// <summary>
    /// Get animation progress
    /// <returns>animation progress</returns>
    public float NormalizedTime()
    {
        //refresh animator state
        stateInfo = enemyModel.animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime;
    }
}
