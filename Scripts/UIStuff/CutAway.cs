using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutAway : MonoBehaviour
{
    public GameObject WinningScene;
    public GameObject LoseScene;
    public GameObject LoadScene;
    public GameObject Prompt;

    public void ActivateWin()
    {
        WinningScene.SetActive(true);
        Prompt.SetActive(true);
    }

    public void ActivateLose()
    {
        LoseScene.SetActive(true);
        Prompt.SetActive(true);
    }

    public void LoadingScreen()
    {
        LoadScene.SetActive(true);
    }

    public void AllDisabled()
    {
        WinningScene.SetActive(false);
        LoseScene.SetActive(false);
        LoadScene.SetActive(false);
        Prompt.SetActive(false);
    }
}
