using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy config info
[CreateAssetMenu(menuName = "Config/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    // Enemy sets
    public GameObject[] models;
}
