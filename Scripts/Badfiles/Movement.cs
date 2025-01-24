using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void OnAnimatorMove()
    {
        // Get the root motion delta from the Animator
        Vector3 deltaPosition = animator.deltaPosition;

        // Move the Character Controller using the root motion delta
        controller.Move(deltaPosition);
    }
}
