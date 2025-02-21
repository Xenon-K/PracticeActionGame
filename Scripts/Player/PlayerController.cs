using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float rotationSpeed = 20f;

    //ult counter
    public int ultCounter = 0;
    //ult limit
    public int maxUlt = 3000;

    //evade cooldown
    private float evadeTimer = 1;

    //evade count
    private int evadeCounter = 0;

    //energyTimer
    private float energyTimer = 0f;

    //state machine
   private StateMachine stateMachine;

    //player config info
    public PlayerConfig playerConfig;

    //character list
    private List<PlayerModel> controllableModels;
    // Add a public property to expose controllableModels
    public List<PlayerModel> ControllableModels => controllableModels;

    [SerializeField] private Collider attackCollider; // Reference to the attack collider

    //playable counter
    public int controllableCounter;

    //current controlling character
    private int currentModelIndex;
    // Add a public property to expose the currentModelIndex
    public int CurrentModelIndex => currentModelIndex;

    public CharacterStats playerStats; // Reference to the player's stats
    public CharacterStats enemyStats; // Reference to the enemy's stats

    // To track the slow-motion coroutine
    private Coroutine globalSlowMotionCoroutine;

    //Ex mark
    public bool canEx = false;

    //Health bar for character
    public List<HealthBar> healthBar;

    //chain attack ui
    public SwitchEffect switchEffect;

    //UI bottons
    public ButtonEffect skillButton;
    public ButtonEffect attackButton;
    public ButtonEffect ultButton;
    public ButtonEffect evadeButton;
    public ButtonEffect switchButton;
    public UltEffect ultEffect;
    //final switch for ui
    public ShotEffect shotPanel;
    //for hit lag
    private float originalFixedDeltaTime;
    //for loading/winning/losing screen
    public CutAway cutAway;
    //for backgroudn musci switch
    public BackgroundMusicManager bkmManager;

    //end game flag
    private bool isGameEnding = false; // Prevent multiple calls

    //hit flag
    public bool isHit = false;
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
            controllableModels[i].gameObject.SetActive(true);
            controllableModels[i].gameObject.SetActive(false);
            healthBar[i].gameObject.SetActive(false);
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
        // Store the original fixed delta time to restore it later
        originalFixedDeltaTime = Time.fixedDeltaTime;
        //if end the game with slow motion last time
        RestoreGlobalTimeScale();
        //lock the mouse
        LockMouse();
        //switch to Idle
        SwitchState(PlayerState.QuestStart);
        //shotPanel.StartAndEndScene();
        #region initialize healthBar
        for (int i = 0; i < playerConfig.models.Length; i++)
        {
            healthBar[i].gameObject.SetActive(true);
            CharacterStats cs = controllableModels[i].GetComponent<CharacterStats>();
            //update health bar
            healthBar[i].SetHealth(cs.maxHealth);
            healthBar[i].SetEnergy(cs.currentEnergy);
        }
        #endregion
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerModel.currentState == PlayerState.NormalAttack)
        {
            //Debug.Log("Detected something");
            var normalAttackState = stateMachine.GetCurrentState<PlayerNormalAttackState>() as PlayerNormalAttackState;
            normalAttackState?.OnAttackHit(other);
        }
        if (playerModel.currentState == PlayerState.BigSkill)
        {
            //Debug.Log("Detected something");
            var bigAttackState = stateMachine.GetCurrentState<PlayerBigSkillState>() as PlayerBigSkillState;
            bigAttackState?.OnAttackHit(other);
        }
        
        if (playerModel.currentState == PlayerState.Attack_Branch)
        {
            //Debug.Log("Detected something");
            var branchAttackState = stateMachine.GetCurrentState<PlayerAttackBranchState>() as PlayerAttackBranchState;
            branchAttackState?.OnAttackHit(other);
        }

        if (playerModel.currentState == PlayerState.Attack_Branch_Walk)
        {
            //Debug.Log("Detected something");
            var branchAttackState = stateMachine.GetCurrentState<PlayerAttackBranchWalkState>() as PlayerAttackBranchWalkState;
            branchAttackState?.OnAttackHit(other);
        }

        if (playerModel.currentState == PlayerState.Attack_Branch_Loop)
        {
            //Debug.Log("Detected something");
            var branchAttackState = stateMachine.GetCurrentState<PlayerAttackBranchLoopState>() as PlayerAttackBranchLoopState;
            branchAttackState?.OnAttackHit(other);
        }

        if (playerModel.currentState == PlayerState.Attack_Branch_Explode)
        {
            //Debug.Log("Detected something");
            var branchAttackState = stateMachine.GetCurrentState<PlayerAttackBranchExplodeState>() as PlayerAttackBranchExplodeState;
            branchAttackState?.OnAttackHit(other);
        }
        if (playerModel.currentState == PlayerState.AttackRush || playerModel.currentState == PlayerState.AttackRushBack)
        {
            //Debug.Log("Detected something");
            var branchAttackState = stateMachine.GetCurrentState<PlayerAttackRushState>() as PlayerAttackRushState;
            branchAttackState?.OnAttackHit(other);
        }
        if (playerModel.currentState == PlayerState.SwitchInAttack)
        {
            //Debug.Log("Detected something");
            var branchAttackState = stateMachine.GetCurrentState<PlayerSwitchInAttackState>() as PlayerSwitchInAttackState;
            branchAttackState?.OnAttackHit(other);
        }
        if (playerModel.currentState == PlayerState.SwitchInAttackEx)
        {
            //Debug.Log("Detected something");
            var branchAttackState = stateMachine.GetCurrentState<PlayerSwitchInAttackExState>() as PlayerSwitchInAttackExState;
            branchAttackState?.OnAttackHit(other);
        }
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
            case PlayerState.Attack_Branch_Perfect_End:
                stateMachine.EnterState<PlayerAttackBranchPerfectEndState>();
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
            case PlayerState.Hit:
                stateMachine.EnterState<PlayerHitState>();
                break;
            case PlayerState.AttackRush:
            case PlayerState.AttackRushBack:
                stateMachine.EnterState<PlayerAttackRushState>();
                break;
            case PlayerState.AttackRushEnd:
            case PlayerState.AttackRushBackEnd:
                stateMachine.EnterState<PlayerAttackRushEndState>();
                break;
            case PlayerState.SwitchInAttack:
                stateMachine.EnterState<PlayerSwitchInAttackState>();
                break;
            case PlayerState.SwitchInAttackEnd:
                stateMachine.EnterState<PlayerSwitchInAttackEndState>();
                break;
            case PlayerState.SwitchInAttackEx:
                stateMachine.EnterState<PlayerSwitchInAttackExState>();
                break;
            case PlayerState.SwitchInAttackExEnd:
                stateMachine.EnterState<PlayerSwitchInAttackExEndState>();
                break;
            case PlayerState.QuestStart:
                stateMachine.EnterState<PlayerQuestStartState>();
                break;
        }
    }

    /// <summary>
    /// switch to next character
    public void SwitchNextModel()
    {
        if (controllableCounter <= 1)
        {
            return;
        }
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
            //Debug.Log("Skip one character");
            currentModelIndex++;
            if (currentModelIndex >= controllableModels.Count)
            {
                currentModelIndex = 0;
            }
            nextModel = controllableModels[currentModelIndex];
        }
        if (nextModel.currentState == PlayerState.Death)
        {
            //Debug.Log("Skip two character");
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
        playerModel.currentState = PlayerState.Idle;//reset state
        playerModel.Enter(prevPos, prevRot);
        //switch state for last character
        SwitchState(PlayerState.SwitchInNormal);
    }

    
    /// <summary>
    /// switch to next character: follow up attack
    public void SwitchNextSupportModel()
    {
        if (controllableCounter <= 1)
        {
            return;
        }
        bool needExit = true;//check if current character need exit
        if (playerStats.currentHealth <= 0)
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
            //Debug.Log("Skip one character");
            currentModelIndex++;
            if (currentModelIndex >= controllableModels.Count)
            {
                currentModelIndex = 0;
            }
            nextModel = controllableModels[currentModelIndex];
        }
        if (nextModel.currentState == PlayerState.Death)
        {
            //Debug.Log("Skip two character");
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
        if (!isShift)
        {
            //Calculate the direction to the right
            Vector3 rightDirection = prevRot * Vector3.right;
            //Shift position to the right by 0.8 units
            prevPos += rightDirection * 0.8f;
        }
        playerModel.currentState = PlayerState.Idle;//reset state
        playerModel.Enter(prevPos, prevRot);
        //switch state for last character
        SwitchState(PlayerState.SwitchInAttack);
    }

    /// <summary>
    /// switch to next character: Ex attack
    public void SwitchNextExModel()
    {
        if (controllableCounter <= 1)
        {
            return;
        }
        bool needExit = true;//check if current character need exit
        if (playerStats.currentHealth <= 0)
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
            //Debug.Log("Skip one character");
            currentModelIndex++;
            if (currentModelIndex >= controllableModels.Count)
            {
                currentModelIndex = 0;
            }
            nextModel = controllableModels[currentModelIndex];
        }
        if (nextModel.currentState == PlayerState.Death)
        {
            //Debug.Log("Skip two character");
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
        if (!isShift)
        {
            // Calculate new position relative to the enemy
            Transform closestEnemy = FindClosestEnemy(20f); // Assumes you have a function to find the closest enemy
            if (closestEnemy != null)
            {
                // Direction from enemy to player
                Vector3 directionFromEnemy = (playerModel.transform.position - closestEnemy.position).normalized;

                // Set the new position 2f away from the enemy
                prevPos = closestEnemy.position + directionFromEnemy * 2f;

                // Add a slight rightward shift based on rotation
                Vector3 rightDirection = prevRot * Vector3.right;
                prevPos += rightDirection * 0.8f; // Adjust this value if needed for better placement
            }
        }
        playerModel.currentState = PlayerState.Idle;//reset state
        playerModel.Enter(prevPos, prevRot);
        //switch state for last character
        SwitchState(PlayerState.SwitchInAttackEx);
    }

    /// <summary>
    /// switch last character
    public void SwitchLastModel()
    {
        //refresh state machine
        //stateMachine.Clear();

        if (controllableCounter <= 1)
        {
            return;
        }

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
        SwitchState(PlayerState.SwitchInNormal);
    }

    /// <summary>
    /// switch last character
    public void SwitchLastSupportModel()
    {
        //refresh state machine
        //stateMachine.Clear();

        if (controllableCounter <= 1)
        {
            return;
        }

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
        SwitchState(PlayerState.SwitchInNormal);
    }

    /// <summary>
    /// switch last character
    public void SwitchLastExModel()
    {
        //refresh state machine
        //stateMachine.Clear();

        if (controllableCounter <= 1)
        {
            return;
        }

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

        // Calculate new position relative to the enemy
        Transform closestEnemy = FindClosestEnemy(20f); // Assumes you have a function to find the closest enemy
        if (closestEnemy != null)
        {
            // Direction from enemy to player
            Vector3 directionFromEnemy = (playerModel.transform.position - closestEnemy.position).normalized;

            // Set the new position 2f away from the enemy
            prevPos = closestEnemy.position + directionFromEnemy * 2f;

            // Add a slight rightward shift based on rotation
            Vector3 rightDirection = prevRot * Vector3.right;
            prevPos += rightDirection * 0.8f; // Adjust this value if needed for better placement
        }

        //enter the last character at this spot
        playerModel.Enter(prevPos, prevRot);
        SwitchState(PlayerState.SwitchInAttackEx);
    }

    /// <summary>
    /// play animation
    /// <param name="animationName"></param>
    /// <param name="fixedTransitionDurarion"></param>
    public int PlayAnimation(string animationName,float fixedTransitionDurarion = 0.25f)
    {
        if (!playerModel.animator.HasState(0, Animator.StringToHash(animationName)))
        {
            Debug.Log($"Animation {animationName} not found in Animator.");
            return -1;
        }
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
        //update player stat
        playerStats = playerModel.GetComponent<CharacterStats>();

        #region update ui bottons
        if (Input.GetKeyDown(KeyCode.U))//for debug ult purpose
        {
            ChargeUlt(maxUlt);
        }
        ultEffect.SetPhase(ultCounter);//change the ult counter image
        //ult
        if (ultCounter == maxUlt)
        {
            ultButton.FullEnergyState();
        }
        else
        {
            ultButton.NormalState();
        }
        //skill
        if(playerStats.currentEnergy >= playerStats.instantUse.GetValue())
        {
            skillButton.FullEnergyState();
        }
        else if (inputSystem.Player.AttackBranch.IsPressed())//pressed skill
        {
            skillButton.PressedButton();
        }
        else
        {
            skillButton.NormalState();
        }
        //attack
        if (inputSystem.Player.Fire.IsPressed())//pressed normal attack
        {
            attackButton.PressedButton();
        }
        else
        {
            attackButton.NormalState();
        }
        //evade
        if(evadeCounter == 2)//evade cooldown
        {
            evadeButton.EvadeCoolDownState((float)System.Math.Round(1f-evadeTimer, 1));
        }
        else if (inputSystem.Player.Evade.IsPressed())//evade is pressed
        {
            evadeButton.PressedButton();
        }
        else
        {
            evadeButton.NormalState();
        }
        //switchdown
        if (checkNextModel())//if next model is not avaliable at the time
        {
            switchButton.FullEnergyState();
        }
        else if (inputSystem.Player.SwitchDown.IsPressed())//pressed switch
        {
            switchButton.PressedButton();
        }
        else
        {
            switchButton.NormalState();
        }
        #endregion

        if (controllableCounter <= 0 && !isGameEnding) 
        {
            isGameEnding = true; // Set flag to prevent multiple calls
            EndGame();
        }
        //update movement input
        inputMoveVec2 = inputSystem.Player.Move.ReadValue<Vector2>().normalized;
        
        #region update game model health
        for (int i = 0; i < playerConfig.models.Length; i++)
        {
            CharacterStats cs = controllableModels[i].GetComponent<CharacterStats>();
            //update health bar
            if (cs.currentHealth <= 0)
                controllableModels[i].currentState = PlayerState.Death;
            healthBar[i].SetHealth(cs.currentHealth);
            //Debug.Log($"{cs.name}, health = {cs.currentHealth}");
        }
        #endregion

        #region update game model energy
        for (int i = 0; i < playerConfig.models.Length; i++)
        {
            CharacterStats cs = controllableModels[i].GetComponent<CharacterStats>();
            //update energy bar
            healthBar[i].SetEnergy(cs.currentEnergy);
            //Debug.Log($"{cs.name}, health = {cs.currentHealth}");
        }
        #endregion

        #region energy recharge
        energyTimer += Time.deltaTime;
        //off field energy charge
        if (energyTimer >= 1f)
        {
            int last = findLastModel();
            int next = findNextModel();
            CharacterStats cs;
            if (last != -1)
            {
                cs = controllableModels[last].GetComponent<CharacterStats>();
                cs.GainEnergy(1);//gain one energy for off field
            }
            if (next != -1)
            {
                cs = controllableModels[next].GetComponent<CharacterStats>();
                cs.GainEnergy(1);//gain one energy for off field
            }
            energyTimer = 0f;
        }
        #endregion

        //recover evade cooldown
        if (evadeTimer < 1f) //start the cooldown no matter which evade number is this
        {
            evadeTimer += Time.deltaTime;
            if (evadeTimer >= 1f) 
            {
                evadeTimer = 1f;
                evadeCounter = 0;
            }
        }

        
        //check for hit animation
        if (isHit && playerStats.currentHealth > 0)
        {
            //switch to Hit state
            //SwitchState(PlayerState.Hit);
            //playerModel.currentState = PlayerState.Idle;
            isHit = false;
            var hitState = stateMachine.GetCurrentState<PlayerHitState>() as PlayerHitState;
            hitState?.OnHitAgain();
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
        RestoreGlobalTimeScale();
        // Ensure LoseScene is enabled
        switchEffect.UIPanel.SetActive(false);
        cutAway.ActivateLose();
        bkmManager.PlayLosingMusic();
        //Debug.Log("Defeated, restart to retry");
        // click to continue
        StartCoroutine(PlayVideoAndWaitForClick());
    }

    // Coroutine to play video and wait for player input
    private IEnumerator PlayVideoAndWaitForClick()
    {
        // Ensure VideoPlayer is enabled and has time to initialize
        yield return new WaitForEndOfFrame();

        // **Optimized Input Handling**
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        Debug.Log("Mouse clicked! Stopping video & switching to Menu...");

        yield return new WaitForSeconds(0.1f);

        UnlockMouse();
        SceneManager.LoadScene("Menu");
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

    public void RotateTowards(Transform target)
    {
        if (target == null) return;

        Vector3 direction = (target.position - playerModel.transform.position).normalized;
        //Debug.Log($"Rotating towards: {target.name}, Direction: {direction}");

        direction.y = 0; // Ensure the rotation only happens on the horizontal plane
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public Transform FindClosestEnemy(float maxDistance)
    {
        Collider[] colliders = Physics.OverlapSphere(playerModel.transform.position, maxDistance);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(playerModel.transform.position, collider.transform.position);
                //Debug.Log($"Found enemy: {collider.name}, Distance: {distance}");
                if (distance < closestDistance && distance <= maxDistance)
                {
                    closestDistance = distance;
                    closestEnemy = collider.transform;
                }
            }
        }
        /*
        if (closestEnemy != null)
        {
            Debug.Log($"Closest enemy: {closestEnemy.name}, Distance: {closestDistance}");
        }
        else
        {
            Debug.LogWarning("No enemies found within range.");
        }
        */
        return closestEnemy;
    }

    //slow down the game
    public void ApplyGlobalSlowMotion(float duration)
    {
        if (globalSlowMotionCoroutine != null)
        {
            // Stop any existing slow-motion coroutine to prevent overlaps
            StopCoroutine(globalSlowMotionCoroutine);
        }
        globalSlowMotionCoroutine = StartCoroutine(GlobalSlowMotionCoroutine(duration));
    }

    private IEnumerator GlobalSlowMotionCoroutine(float duration)
    {
        float originalTimeScale = Time.timeScale; // Save the original time scale
        Time.timeScale = 0.2f; // Slow everything down by 80%
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixedDeltaTime for physics consistency

        yield return new WaitForSecondsRealtime(duration); // Wait for the slow motion duration (unaffected by time scale)

        //Time.timeScale = originalTimeScale; // Restore the original time scale
        //Time.fixedDeltaTime = 0.02f; // Reset fixedDeltaTime
        // Restore time scale after the duration
        canEx = false;
        DisableChainUI();
        RestoreGlobalTimeScale();
    }

    /// <summary>
    /// Restore the global time scale to normal immediately.
    public void RestoreGlobalTimeScale()
    {
        if (globalSlowMotionCoroutine != null)
        {
            // Stop the current coroutine if it is running
            StopCoroutine(globalSlowMotionCoroutine);
            globalSlowMotionCoroutine = null;
        }

        // Reset the global time scale to normal
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Restore fixedDeltaTime
    }

    /// <summary>
    /// Apply hit lag effect.
    public void ApplyHitLag(float lagDuration)
    {
        StartCoroutine(HitLagCoroutine(lagDuration));
    }

    private IEnumerator HitLagCoroutine(float lagDuration)
    {
        // Slow down time
        Time.timeScale = 0.1f; // Extremely slow (10% of normal speed)
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

        // Wait for the duration of the lag
        yield return new WaitForSecondsRealtime(lagDuration);

        // Restore normal time scale
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }

    public void BroadcastCurrentOrder()
    {
        // Check if controllableModels has any characters
        if (controllableModels == null || controllableModels.Count == 0)
        {
            Debug.LogWarning("No controllable models available.");
            return;
        }

        // Determine current, last, and next character indices
        int lastModelIndex = (currentModelIndex - 1 + controllableModels.Count) % controllableModels.Count;
        int nextModelIndex = (currentModelIndex + 1) % controllableModels.Count;

        // Get references to the characters
        PlayerModel currentCharacter = controllableModels[currentModelIndex];
        PlayerModel lastCharacter = controllableModels[lastModelIndex];
        PlayerModel nextCharacter = controllableModels[nextModelIndex];

        // Log the information
        Debug.Log($"[Broadcast Order] Current Character: {currentCharacter.name}, " +
                  $"Last Character: {lastCharacter.name}, " +
                  $"Next Character: {nextCharacter.name}");
    }

    //check if the next model is active or not
    public bool checkNextModel()
    {
        int nextModelIndex = (currentModelIndex + 1) % playerConfig.models.Length;
        //Debug.Log($"Current next character is {controllableModels[nextModelIndex]?.name}, Active: {controllableModels[nextModelIndex].gameObject.activeSelf}, Index : {nextModelIndex}");
        //Debug.Log($"Current next character is {controllableModels[currentModelIndex]?.name}, Index : {currentModelIndex}");
        if (controllableModels[nextModelIndex].gameObject.activeSelf)
            return true; else return false;
    }

    //find the next inactive model
    public int findNextModel()
    {
        int nextModelIndex = (currentModelIndex + 1) % playerConfig.models.Length;
        if (controllableModels[nextModelIndex].gameObject.activeSelf)
            return -1;
        else return nextModelIndex;
    }

    //check if the last model is active or not
    public bool checkLastModel()
    {
        int lastModelIndex = (currentModelIndex - 1);
        if (lastModelIndex < 0)
        {
            lastModelIndex = playerConfig.models.Length - 1;
        }
        if (controllableModels[lastModelIndex].gameObject.activeSelf)
            return true;
        else return false;
    }

    //find the last inactive model
    public int findLastModel()
    {
        int lastModelIndex = (currentModelIndex - 1);
        if (lastModelIndex < 0)
        {
            lastModelIndex = playerConfig.models.Length - 1;
        }
        if (controllableModels[lastModelIndex].gameObject.activeSelf)
            return -1;
        else return lastModelIndex;
    }

    private void UnlockMouse()
    {
        //unlock the mouse
        Cursor.lockState = CursorLockMode.None;
        //visible
        Cursor.visible = true;
    }

    //callChainAttackUI
    public void ChainUI()
    {
        bool leftCheck = false;
        bool rightCheck = false;
        switchEffect.gameObject.SetActive(true);
        switchEffect.UIPanel.SetActive(false);
        int nextModelIndex = (currentModelIndex + 1) % playerConfig.models.Length;
        CharacterStats cs = controllableModels[nextModelIndex].GetComponent<CharacterStats>();
        if (cs.currentHealth <= 0)//if next character is defeated
        {
            //Debug.Log("not next");
            nextModelIndex = currentModelIndex;
        }
        //check for last model index
        int lastModelIndex = currentModelIndex - 1;
        if (lastModelIndex < 0)
        {
            lastModelIndex = playerConfig.models.Length - 1;
        }

        //if last character is defeated
        cs = controllableModels[lastModelIndex].GetComponent<CharacterStats>();
        if (cs.currentHealth <= 0)
        {
            //Debug.Log("not last");
            lastModelIndex = currentModelIndex;
        }
        if (nextModelIndex == currentModelIndex)
        {
            leftCheck = true;
        }
        if(lastModelIndex == currentModelIndex)
        {
            rightCheck = true;
        }
        switchEffect.AssignBothImage(nextModelIndex, lastModelIndex, checkNextModel(), checkLastModel(), leftCheck, rightCheck);
    }

    public void DisableChainUI()
    {
        switchEffect.UIPanel.SetActive(true);
        switchEffect.gameObject.SetActive(false);
    }

    //allow enter switchInAttackEx doing external ways
    public void EnterSwitchInAttackEx(EnemyController enemyController)
    {
        if (enemyController.exChance > 0)
        {
            enemyController.usedExChance();
            canEx = true;
            ApplyGlobalSlowMotion(3f);
            BroadcastCurrentOrder();
            ChainUI();
        }
    }

    public void ChargeUlt(int charge)
    {
        if (ultCounter + charge > maxUlt)
            ultCounter = maxUlt;
        else if(ultCounter + charge <= 0)
            ultCounter = 0;
        else
            ultCounter += charge;
    }

    public bool CheckUlt()
    {
        if (ultCounter == maxUlt)
            return true;
        else
            return false;
    }

    public void UsedUlt()
    {
        ultCounter = 0;
    }
}
