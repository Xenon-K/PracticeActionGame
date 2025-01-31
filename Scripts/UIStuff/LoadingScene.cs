using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public CutAway cutAway;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        // Activate the loading screen
        cutAway.LoadScene.SetActive(true);

        // Start loading the scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            yield return null;
        }

        // Deactivate the loading screen after loading is complete
        cutAway.LoadScene.SetActive(false);
    }
}