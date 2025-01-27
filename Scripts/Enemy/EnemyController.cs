using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : SingleMonoBaseEnemy<EnemyController>, IStateMachineOwner
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
    public CharacterStats playerStats; // Reference to the player's stats
    public CharacterStats enemyStats; // Reference to the enemy's stats
    public PlayerController playerController;
    public PlayerModel targetPlayerModel;

    [SerializeField] private Collider attackCollider; // Reference to the attack collider

    //state machine
    private StateMachine stateMachine;

    //player model
    public EnemyModel enemyModel;

    private float originalAgentSpeed; // Store original NavMeshAgent speed
    private float originalAnimatorSpeed; // Store original animator speed

    public int exChance = 0;//the chances of player can perform ex attack on this enemy
    public float exTimer = 0f;//ex attack timer

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine(this);

        enemyStats = GetComponent<CharacterStats>();

        #region debug find player info section
        if (PlayerManager.instance == null)
            Debug.Log("PlayerManager.instance");
        if (PlayerManager.instance.player == null)
            Debug.Log("PlayerManager.instance.player");
        if (PlayerManager.instance.player.transform == null)
            Debug.Log("PlayerManager.instance.player.transform");
        #endregion

        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 8;
        agent.stoppingDistance = 2;
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();

        originalAgentSpeed = agent.speed;
        originalAnimatorSpeed = animator.speed;

        #region debug get player info section
        if (playerController != null)
        {
            playerModels = playerController.ControllableModels;
            Debug.Log("EnemyController: Successfully retrieved controllable player models.");
        }
        else
        {
            Debug.LogError("EnemyController: Could not find PlayerController!");
        }
        #endregion

        if (attackCollider != null)
        {
            attackCollider.enabled = false; // Ensure it's initially disabled
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        SwitchState(EnemyState.Born); // Switch to the Born state
        ApplyFreeze(9f);
    }

    /// <summary>
    /// Apply freeze motion to the enemy for a specified duration.
    /// <param name="duration">Duration of the freeze effect in seconds.</param>
    public void ApplyFreeze(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        SetTimeScale(0f); // Freeze
        yield return new WaitForSecondsRealtime(duration);
        SetTimeScale(1f); // Reset to normal speed
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
            case EnemyState.Stun_Start:
                stateMachine.EnterState<EnemyStunStartState>();
                break;
            case EnemyState.Stun_Loop:
                stateMachine.EnterState<EnemyStunState>();
                break;
            case EnemyState.Stun_End:
                stateMachine.EnterState<EnemyStunEndState>();
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
        /*
        // Ensure PlayerManager is initialized
        if (target == null && PlayerManager.instance?.player?.transform != null)
        {
            target = PlayerManager.instance.player.transform;
            Debug.Log("EnemyController: Target successfully assigned during runtime.");
        }*/

        //if the list is empty
        if (playerModels == null || playerModels.Count == 0) return;

        // Get the current target based on currentModelIndex
        int currentModelIndex = playerController.CurrentModelIndex;

        // Calculate the distance to the target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Check if the target is within the lookRadius
        if (distanceToTarget > lookRadius || 
            enemyModel.currentState == EnemyState.Born || //born state, enemy don't move
            enemyModel.currentState == EnemyState.NormalAttack || //Attack state
            enemyModel.currentState == EnemyState.Death || //got defeated
            enemyModel.currentState == EnemyState.Stun_Start || //all stun stages
            enemyModel.currentState == EnemyState.Stun_Loop ||
            enemyModel.currentState == EnemyState.Stun_End
            )
        {
            StopAgentMovement();
            return; // Exit the update method to prevent further processing
        }
        else
        {
            ResumeAgentMovement();
            agent.SetDestination(target.position); // Ensure the agent moves toward the target
        }

        if (playerModels != null && currentModelIndex >= 0 && currentModelIndex < playerModels.Count)
        {
            targetPlayerModel = playerModels[currentModelIndex];
            target = targetPlayerModel.transform;
            agent.SetDestination(target.position); // Ensure the agent is using the correct target
        }

        if (exChance > 0 && exTimer < 5f) //start the cooldown no matter which evade number is this
        {
            exTimer += Time.deltaTime;
            if (exTimer >= 5f)
            {
                restoreExChance();
            }
        }

        playerStats = targetPlayerModel.GetComponent<CharacterStats>();
        //Debug.Log($"EnemyController: Current target is {target?.name}, Position: {target?.position}");
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

    //stop enemy ai
    public void StopAgentMovement()
    {
        if (agent != null)
        {
            agent.isStopped = true; // Prevent the agent from moving
            agent.velocity = Vector3.zero; // Reset its velocity
        }
    }

    //resume enemy ai
    public void ResumeAgentMovement()
    {
        if (agent != null)
        {
            agent.isStopped = false; // Allow the agent to move again
        }
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

    /// <summary>
    /// Adjust the enemy's speed and animation speed for slow motion.
    /// <param name="timeScale">The time scale to apply (e.g., 0.5 for half speed).</param>
    public void SetTimeScale(float timeScale)
    {
        agent.speed = originalAgentSpeed * timeScale;
        animator.speed = originalAnimatorSpeed * timeScale;

        //Debug.Log($"EnemyController: Slow motion applied with time scale {timeScale}");
    }

    /// <summary>
    /// Apply slow motion to the enemy for a specified duration.
    /// <param name="duration">Duration of the slow motion effect in seconds.</param>
    public void ApplySlowMotion(float duration)
    {
        StartCoroutine(SlowMotionCoroutine(duration));
    }

    private IEnumerator SlowMotionCoroutine(float duration)
    {
        SetTimeScale(0.5f); // Slow down by 50%
        yield return new WaitForSecondsRealtime(duration);
        SetTimeScale(1f); // Reset to normal speed
    }

    public void PlayerExChance()
    {
        exChance = 3;//three chances
    }

    public void usedExChance()
    {
        exChance--;//using one chance
        exTimer = 0f;
    }

    public void restoreExChance()
    {
        exTimer = 0f;
        exChance = 0;
    }
}
