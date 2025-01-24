using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectorManager : MonoBehaviour
{

    #region Singleton
    public static PlayerSelectorManager instance;
    void Awake() { instance = this; }
    #endregion

    // These should be public to show up in the Inspector
    public GameObject playerSelector;

    // Update the playerSelector dynamically
    public void UpdatePlayerSelector(GameObject newSelector)
    {
        playerSelector = newSelector;
    }
}
