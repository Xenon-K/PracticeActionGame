using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player config info
[CreateAssetMenu(menuName = "Config/Player Config")] 
public class PlayerConfig : ScriptableObject
{
    // Player team sets
    public GameObject[] models;
}
