using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    public Transform Target;
    //height
    private float height;

    private void Awake()
    {
        height = transform.position.y;
    }

    private void LateUpdate()
    {
        Vector3 playerPos = Target.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + height, playerPos.z);
    }
}
