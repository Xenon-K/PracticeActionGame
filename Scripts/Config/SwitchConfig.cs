using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Switch config for character image
[CreateAssetMenu(menuName = "Config/Switch Config")]
public class SwitchConfig : ScriptableObject
{
    // Player team sets
    public Sprite[] models;
}
