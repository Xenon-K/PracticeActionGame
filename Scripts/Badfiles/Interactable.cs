using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;

    bool isFocus = false;
    Transform player;

    bool hasInteracted = false;

    public virtual void Interact()
    {
        //overwritten
        Debug.Log("Interacting with " + transform.name);
    }

    // Update is called once per frame
    void Update()
    {
        if(isFocus && !hasInteracted) 
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if(distance <= radius)
            {
                Interact();
                hasInteracted = true;
            }
        }
    }

    public void OnFocused (Transform playerTransform)
    {
        isFocus = true;
        player = playerTransform;
    }
}
