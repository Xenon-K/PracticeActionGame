using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// skill config
[CreateAssetMenu(menuName = "Config/Skill")] 
public class SkiilConfig : ScriptableObject
{
    //current normal attack stage
    [HideInInspector]public int currentNormalAttackIndex = 1;

    //more branch normal attack or not
    [HideInInspector] public bool isPerfect = false;

    //input buffer for combo moves
    [HideInInspector] public bool HasComboed { get; set; }

    //input buffer for combo moves
    [HideInInspector] public bool HoldingComboed { get; set; }

    //Normal attack damage multiplier per stage
    public float[] normalAttackDamageMultiple;
}
