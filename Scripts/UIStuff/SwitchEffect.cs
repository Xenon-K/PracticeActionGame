using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchEffect : MonoBehaviour
{
    public Image Left; // Reference to the chain attack
    public Image Right;
    public GameObject Left_Corss; // If this character is off field
    public GameObject Right_Corss;
    public SwitchConfig config; // Reference to the ScriptableObject
    public GameObject UIPanel;

    public void AssignBothImage(int left_index, int right_index, bool left_visibility, bool right_visibility)
    {
        Left_Corss.SetActive(false);
        Right_Corss.SetActive(false);
        Left.sprite = config.models[left_index];
        Right.sprite = config.models[right_index];
        if(left_visibility)
        {
            Left_Corss.SetActive(true);
        }
        if (right_visibility)
        {
            Right_Corss.SetActive(true);
        }
    }
}

