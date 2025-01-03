using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    //animator
    [HideInInspector] public Animator animator;
    //enemy current state
    public EnemyState currentState;
    //character controller
    [HideInInspector] public EnemyController enemyController;
    //animator info
    private AnimatorStateInfo stateInfo;
    //switch out timer
    private float animationTimer = 0f; // Tracks how long the animation has been playing

    private void Awake()
    {
        if (enemyController != null)
        {
            enemyController.enemyModel = this;
        }
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
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
}
