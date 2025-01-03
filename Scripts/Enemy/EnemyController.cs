using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : SingleMomoBase<EnemyController>, IStateMachineOwner
{
    //search radius
    public float lookRadius = 10f;
    private List<PlayerModel> playerModels;

    //ai movement
    public Vector2 inputMoveVec2;

    Transform target; // Current player target
    NavMeshAgent agent;

    public Transform Target => target; // Expose target as a public property
    public NavMeshAgent Agent => agent; // Expose agent as a public property

    Animator animator;
    [HideInInspector] public CharacterStats playerStats; // Reference to the player's stats
    [HideInInspector] public CharacterStats enemyStats; // Reference to the enemy's stats
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public PlayerModel targetPlayerModel;

    [SerializeField] private Collider attackCollider; // Reference to the attack collider

    //state machine
    private StateMachine stateMachine;

    //player model
    public EnemyModel enemyModel;

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine(this);

        enemyStats = GetComponent<CharacterStats>();
        
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 5;
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            playerModels = playerController.ControllableModels;
            Debug.Log("EnemyController: Successfully retrieved controllable player models.");
        }
        else
        {
            Debug.LogError("EnemyController: Could not find PlayerController!");
        }
        if (attackCollider != null)
        {
            attackCollider.enabled = false; // Ensure it's initially disabled
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //switch to Idle
        SwitchState(EnemyState.Born);
    }

    /// <summary>
    /// switch state
    public void SwitchState(EnemyState enemyState)
    {
        enemyModel.currentState = enemyState;
        switch (enemyState)
        {
            case EnemyState.Idle:
                stateMachine.EnterState<EnemyIdleState>();
                break;
            case EnemyState.Walk:
                stateMachine.EnterState<EnemyWalkState>();
                break;
            case EnemyState.NormalAttack:
                stateMachine.EnterState<EnemyNormalAttackState>();
                break;
            case EnemyState.Born:
                stateMachine.EnterState<EnemyBornState>();
                break;
            case EnemyState.Death:
                stateMachine.EnterState<EnemyDeathState>();
                break;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (enemyModel.currentState == EnemyState.NormalAttack)
        {
            var normalAttackState = stateMachine.GetCurrentState<EnemyNormalAttackState>() as EnemyNormalAttackState;
            normalAttackState?.OnAttackHit(other);
        }
    }

    /// <summary>
    /// play animation
    public int PlayAnimation(string animationName, float fixedTransitionDurarion = 0.25f)
    {
        enemyModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDurarion);
        return 0;
    }

    /*
    private void FixedUpdate()
    {
        
        //update movement input
        inputMoveVec2 = inputSystem.Player.Move.ReadValue<Vector2>().normalized;
        //recover evade cooldown
        if (evadeTimer < 1f) //start the cooldown no matter which evade number is this
        {
            evadeTimer += Time.deltaTime;
            if (evadeTimer > 1f)
            {
                evadeTimer = 1f;
                evadeCounter = 0;
            }
        }
        
    }*/

    // Update is called once per frame
    void Update()
    {

        //if the list is empty
        if (playerModels == null || playerModels.Count == 0) return;

        // Get the current target based on currentModelIndex
        int currentModelIndex = playerController.CurrentModelIndex;

        if (playerModels != null && currentModelIndex >= 0 && currentModelIndex < playerModels.Count)
        {
            targetPlayerModel = playerModels[currentModelIndex];
        }

        playerStats = targetPlayerModel.GetComponent<CharacterStats>();
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 6f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }


    // Enable attack collider (called by animation event)
    public void EnableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }

    // Disable attack collider (called by animation event)
    public void DisableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }
}
