using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public int level;
    
    public void OpenScene()
    {
        SceneManager.LoadScene("SinglePractice " + level);
    }

    public void TestScene()
    {
        Debug.Log("Loading");
    }
}
