using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public enum ModelFoot
{
    Left, Right
}
public class PlayerModel : MonoBehaviour
{
    //animator
    [HideInInspector] public Animator animator;
    //player current state
    public PlayerState currentState;
    //character controller
    [HideInInspector] public CharacterController characterController;
    //animator info
    private AnimatorStateInfo stateInfo;
    //gravity
    public float gravity = -9.8f;
    //skill config
    public SkiilConfig skiilConfig;
    //ult start shot
    public GameObject bigSkillStartShot;
    //ult shot
    public GameObject bigSkillShot;
    //switch out timer
    private float animationTimer = 0f; // Tracks how long the animation has been playing
    // Prevent re-triggering exit logic
    private bool isExiting = false;
    // check for exit position logic
    private bool isAttack = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    #region animation state
    public ModelFoot foot = ModelFoot.Left;

    /// <summary>
    /// Validates and adjusts the position to ensure it is on valid ground
    public Vector3 ValidateGroundPosition(Vector3 targetPos)
    {
        // Define the maximum distance to check below the target position
        float groundCheckDistance = 5f;

        // Perform a raycast downward to check for ground
        if (Physics.Raycast(targetPos, Vector3.down, out RaycastHit hitInfo, groundCheckDistance))
        {
            // Adjust the target position to the ground level
            return hitInfo.point;
        }

        // If no ground is detected, return the original position (fallback)
        return targetPos;
    }

    /// <summary>
    /// animation state
    public void Enter(Vector3 pos, Quaternion rot)
    {
        // Remove the exit logic from the update manager
        MonoManager.INSTANCE.RemoveUpdateAction(OnExit);

        #region character enter point
        //Calculate the direction to the right
        //Vector3 rightDirection = rot * Vector3.right;
        //Shift position to the right by 0.8 units
        //pos += rightDirection * 0.8f;
        //Calculate the direction to the back
        Vector3 backDirection = rot * Vector3.back;
        //Shift position to the back by 4 units
        pos += backDirection * 4f;

        // Ensure the spawn position is valid (adjust position to the ground if necessary)
        pos = ValidateGroundPosition(pos);

        characterController.Move(pos - transform.position);
        transform.rotation = rot;
        #endregion
        
    }

    /// <summary>
    /// model exit
    public bool Exit(Vector3 pos, Quaternion rot, bool needExit) 
    {
        if(!needExit)
        {
            MonoManager.INSTANCE.AddUpdateAction(OnExit);
            return true;
        }
        // If already exiting, prevent further calls
        if (isExiting) return false;

        // Check if the current action/animation is complete
        if (!IsCurrentActionComplete())
        {
            // Wait until the current action is complete
            MonoManager.INSTANCE.AddUpdateAction(WaitForActionToComplete);
            return false;
        }

        // Mark as exiting
        isExiting = true;

        animator.CrossFade("SwitchOut_Normal", 0.1f);
        MonoManager.INSTANCE.AddUpdateAction(OnExit);
        if (!isAttack) 
        {
            #region character exit point
            //Calculate the direction to the left
            Vector3 leftDirection = rot * Vector3.left;
            //Shift position to the right by 0.8 units
            pos += leftDirection * 0.8f;
            //Calculate the direction to the back
            Vector3 backDirection = rot * Vector3.back;
            //Shift position to the back by 1 units
            pos += backDirection * 1f;

            // Ensure the spawn position is valid (adjust position to the ground if necessary)
            pos = ValidateGroundPosition(pos);

            characterController.Move(pos - transform.position);
            transform.rotation = rot;
            #endregion
        }
        //done exit
        isExiting = false;
        isAttack = false;
        return true;
    }

    /// <summary>
    /// remove action after animation is done
    private void WaitForActionToComplete()
    {
        // Check if the current action is done
        if (IsCurrentActionComplete())
        {
            MonoManager.INSTANCE.RemoveUpdateAction(WaitForActionToComplete); // Stop waiting
            Exit(transform.position, transform.rotation, true); // Retry the exit logic
        }
    }

    /// <summary>
    /// remove action after animation is done
    private bool IsCurrentActionComplete()
    {
        // Do not want to make all actions stay, such as run
        if ((currentState == PlayerState.NormalAttack || currentState == PlayerState.Attack_Branch || currentState == PlayerState.Attack_Branch_Walk || currentState == PlayerState.Attack_Branch_Loop) && !IsAnimationEnd())
        {
            isAttack = true;
            return false; // Action not yet complete
        }

        return true; // Action is complete, or does not need to stop
    }

    /// <summary>
    /// exit logic
    public void OnExit()
    {
        animationTimer += Time.deltaTime; // Increment the timer
        #region Check if the animation has finished playing
        if (IsAnimationEnd() || animationTimer >= 2f)
        {
            gameObject.SetActive(false);
            MonoManager.INSTANCE.RemoveUpdateAction(OnExit);

            // Reset the timer
            animationTimer = 0f;
        }
        #endregion
    }

    /// <summary>
    /// Determine if the animation has finished
    public bool IsAnimationEnd()
    {
        //Refresh animation state
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1.0f && !animator.IsInTransition(0);
    }

    /// <summary>
    /// Set exit with the left foot
    public void SetOutLeftFoot()
    {
        foot = ModelFoot.Left;
    }

    /// <summary>
    /// Set exit with the right foot
    public void SetOutRightFoot()
    {
        foot = ModelFoot.Right;
    }

    private void OnDisable()
    {
        //Reset normal attack index
        skiilConfig.currentNormalAttackIndex = 1;
    }
    #endregion
}
