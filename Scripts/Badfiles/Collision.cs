using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.name == "Monster_Giant (merge)")
        {
            Debug.Log("Enter: " + gameObject.name);
            animator.applyRootMotion = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Monster_Giant (merge)")
        {
            Debug.Log("Stay: " + gameObject.name);
            animator.applyRootMotion = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Monster_Giant (merge)")
        {
            Debug.Log("Exit: " + gameObject.name);
            animator.applyRootMotion = true;
        }
    }
}
