using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// gamer controller
public class PlayerController : SingleMomoBase<PlayerController>,IStateMachineOwner
{
    //Input system
    [HideInInspector]public InputSystem inputSystem;
    //movement input
    public Vector2 inputMoveVec2;
    
    //player model
    public PlayerModel playerModel;

    //model rotation speed
    public float rotationSpeed = 8f;

    //evade cooldown
    private float evadeTimer = 1;

    //evade count
    private int evadeCounter = 0;

    //state machine
    private StateMachine stateMachine;

    //player config info
    public PlayerConfig playerConfig;

    //character list
    private List<PlayerModel> controllableModels;
    // Add a public property to expose controllableModels
    public List<PlayerModel> ControllableModels => controllableModels;

    //playable counter
    public int controllableCounter;

    //current controlling character
    private int currentModelIndex;
    // Add a public property to expose the currentModelIndex
    public int CurrentModelIndex => currentModelIndex;

    public CharacterStats playerStats; // Reference to the player's stats

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new StateMachine(this);
        inputSystem= new InputSystem();

        controllableModels = new List<PlayerModel>();
        controllableCounter = playerConfig.models.Length;

        #region create game models
        for (int i = 0; i < playerConfig.models.Length; i++) 
        {
            GameObject modle = Instantiate(playerConfig.models[i], transform);
            controllableModels.Add(modle.GetComponent<PlayerModel>());
            controllableModels[i].gameObject.SetActive(false);
        }
        #endregion

        #region first character
        currentModelIndex = 0;
        controllableModels[currentModelIndex].gameObject.SetActive(true);
        playerModel = controllableModels[currentModelIndex];
        #endregion
    }

    private void Start()
    {
        //lock the mouse
        LockMouse();
        //switch to Idle
        SwitchState(PlayerState.Idle);
    }

    /// <summary>
    /// switch state
    /// <param name="playerState"></param>
    public void SwitchState(PlayerState playerState)
    {
        playerModel.currentState= playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Idle_AFK:
                stateMachine.EnterState<PlayerIdleState>(true);
                break;
            case PlayerState.Walk:
            case PlayerState.Run:
                stateMachine.EnterState<PlayerRunState>(true);
                break;
            case PlayerState.RunEnd:
                stateMachine.EnterState<PlayerRunEndState>();
                break;
            case PlayerState.TurnBack:
                stateMachine.EnterState<PlayerTurnBackState>();
                break;
            case PlayerState.Evade_Front:
            case PlayerState.Evade_Back:
                if (evadeCounter >= 2)//just did two evade
                {
                    return;
                }
                stateMachine.EnterState<PlayerEvadeState>();
                evadeTimer = 0f;
                evadeCounter ++;
                break;
            case PlayerState.Evade_Front_End:
            case PlayerState.Evade_Back_End:
                stateMachine.EnterState<PlayerEvadeEndState>();
                break;
            case PlayerState.NormalAttack:
                stateMachine.EnterState<PlayerNormalAttackState>(true);
                break;
            case PlayerState.NormalAttackEnd:
                stateMachine.EnterState<PlayerNormalAttackEndState>();
                break;
            case PlayerState.BigSkillStart:
                stateMachine.EnterState<PlayerBigSkillStartState>();
                break;
            case PlayerState.BigSkill:
                stateMachine.EnterState<PlayerBigSkillState>();
                break;
            case PlayerState.BigSkillTransition:
                stateMachine.EnterState<PlayerBigSkillTransitionState>();
                break;
            case PlayerState.BigSkillEnd:
                stateMachine.EnterState<PlayerBigSkillEndState>();
                break;
            case PlayerState.SwitchInNormal:
                stateMachine.EnterState<PlayerSwitchInNormalState>();
                break;
            case PlayerState.Attack_Branch:
                stateMachine.EnterState<PlayerAttackBranchState>(true);
                break;
            case PlayerState.Attack_Branch_End:
                stateMachine.EnterState<PlayerAttackBranchEndState>();
                break;
            case PlayerState.Attack_Branch_Loop:
                stateMachine.EnterState<PlayerAttackBranchLoopState>();
                break;
            case PlayerState.Attack_Branch_Walk:
                stateMachine.EnterState<PlayerAttackBranchWalkState>();
                break;
            case PlayerState.Attack_Branch_Explode:
                stateMachine.EnterState<PlayerAttackBranchExplodeState>();
                break;
            case PlayerState.Death:
                stateMachine.EnterState<PlayerDeathState>();
                break;
        }
    }

    /// <summary>
    /// switch to next character
    public void SwitchNextModel()
    {
        bool needExit = true;//check if current character need exit
        if(playerStats.currentHealth <= 0)
        {
            needExit = false;
        }
        #region control next model
        currentModelIndex++;
        if (currentModelIndex >= controllableModels.Count) 
        {
            currentModelIndex = 0;
        }
        PlayerModel nextModel = controllableModels[currentModelIndex];

        #region detect if it is a character with hp
        if (nextModel.currentState == PlayerState.Death)
        {
            currentModelIndex++;
            if (currentModelIndex >= controllableModels.Count)
            {
                currentModelIndex = 0;
            }
            nextModel = controllableModels[currentModelIndex];
        }
        if (nextModel.currentState == PlayerState.Death)
        {
            return;
        }
        #endregion

        if (nextModel.gameObject.activeInHierarchy == true)
        {
            currentModelIndex--;
            if (currentModelIndex < 0)
            {
                currentModelIndex = controllableModels.Count - 1;
            }
            return;
        }
        //refresh state machine
        stateMachine.Clear();

        nextModel.gameObject.SetActive(true);

        Vector3 prevPos = playerModel.transform.position;
        Quaternion prevRot = playerModel.transform.rotation;

        #endregion
        prevPos = playerModel.ValidateGroundPosition(prevPos);
        //current model turn off
        bool isShift = playerModel.Exit(prevPos, prevRot, needExit);

        playerModel = nextModel;

        //enter the next character at this spot
        if(!isShift)
        {
            //Calculate the direction to the right
            Vector3 rightDirection = prevRot * Vector3.right;
            //Shift position to the right by 0.8 units
            prevPos += rightDirection * 0.8f;
            //Calculate the direction to the back
            //Vector3 backDirection = prevRot * Vector3.back;
            //Shift position to the back by 4 units
            //prevPos += backDirection * 4f;
        }
        playerModel.Enter(prevPos, prevRot);
        //switch state for next character
        SwitchState(PlayerState.SwitchInNormal);
    }

    /// <summary>
    /// switch last character
    public void SwitchLastModel()
    {
        //refresh state machine
        //stateMachine.Clear();
        bool needExit = true;//check if current character need exit
        if (playerStats.currentHealth <= 0)
        {
            needExit = false;
        }
        #region control last model
        currentModelIndex--;
        if (currentModelIndex < 0)
        {
            currentModelIndex = controllableModels.Count - 1;
        }
        PlayerModel nextModel = controllableModels[currentModelIndex];

        #region detect if it is a character with hp
        if (nextModel.currentState == PlayerState.Death)
        {
            currentModelIndex--;
            if (currentModelIndex < 0)
            {
                currentModelIndex = controllableModels.Count - 1;
            }
            nextModel = controllableModels[currentModelIndex];
        }
        if (nextModel.currentState == PlayerState.Death)
        {
            return;
        }
        #endregion

        if (nextModel.gameObject.activeInHierarchy == true)
        {
            currentModelIndex++;
            if (currentModelIndex >= controllableModels.Count)
            {
                currentModelIndex = 0;
            }
            return;
        }
        //refresh state machine
        stateMachine.Clear();

        nextModel.gameObject.SetActive(true);

        Vector3 prevPos = playerModel.transform.position;
        Quaternion prevRot = playerModel.transform.rotation;

        #endregion
        prevPos = playerModel.ValidateGroundPosition(prevPos);
        //current model turn off
        bool isShift = playerModel.Exit(prevPos, prevRot, needExit);

        playerModel = nextModel;

        //enter the next character at this spot
        if (!isShift)
        {
            //Calculate the direction to the right
            Vector3 rightDirection = prevRot * Vector3.right;
            //Shift position to the right by 0.8 units
            prevPos += rightDirection * 0.8f;
            //Calculate the direction to the back
            //Vector3 backDirection = prevRot * Vector3.back;
            //Shift position to the back by 4 units
            //prevPos += backDirection * 4f;
        }

        //enter the last character at this spot
        playerModel.Enter(prevPos, prevRot);
        //switch state for last character
        SwitchState(PlayerState.SwitchInNormal);
    }
    

    /// <summary>
    /// play animation
    /// <param name="animationName"></param>
    /// <param name="fixedTransitionDurarion"></param>
    public int PlayAnimation(string animationName,float fixedTransitionDurarion = 0.25f)
    {
        if (!playerModel.animator.HasState(0, Animator.StringToHash(animationName)))
            return -1;
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDurarion);
        return 0;
    }
    /// <summary>
    /// Play animation, time offset version
    /// <param name="animationName"></param>
    /// <param name="fixedTransitionDurarion"></param>
    /// <param name="fixedTimeOffset"></param>
    public void PlayAnimation(string animationName, float fixedTransitionDurarion = 0.25f, float fixedTimeOffset = 0f)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDurarion, 0, fixedTimeOffset);
    }

    private void FixedUpdate()
    {
        if(controllableCounter <= 0) 
        { 
            EndGame();
        }
        //update movement input
        inputMoveVec2 = inputSystem.Player.Move.ReadValue<Vector2>().normalized;
        //update player stats
        playerStats = controllableModels[CurrentModelIndex].GetComponent<CharacterStats>();
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
        //check for health
        if(playerStats.currentHealth <= 0)
        {
            //switch to Death state
            SwitchState(PlayerState.Death);
        }
    }

    private void LockMouse()
    {
        //lock the mouse in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
        //invisible
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputSystem.Enable();
    }
    private void OnDisable()
    {
        inputSystem.Disable();
    }

    public void EndGame()
    {
        Debug.Log("Defeated, restart to retry");
    }
}
