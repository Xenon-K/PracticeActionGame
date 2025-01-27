using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    //character list
    public List<EnemyController> controllableModels;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        if(IsEndGame())
        {
            EndGame();
            return;
        }
    }

    //check if we enter end game
    public bool IsEndGame()
    {
        for(int i = 0; i < controllableModels.Count; i++)
        {
            if (controllableModels[i].gameObject.activeSelf)//if there is still one enemy left
                return false;
        }
        return true;
    }

    //back to menu
    public void EndGame()
    {
        Debug.Log("You Win!");
        UnlockMouse();
        SceneManager.LoadScene("Menu");
    }

    private void UnlockMouse()
    {
        //unlock the mouse
        Cursor.lockState = CursorLockMode.None;
        //visible
        Cursor.visible = true;
    }
}
