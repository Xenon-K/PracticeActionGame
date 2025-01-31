using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    //end game flag
    private bool EndGameChecked = false;
    public SwitchEffect switchEffect;
    public CutAway cutAway;
    //character list
    public List<EnemyController> controllableModels;
    //for backgroudn musci switch
    public BackgroundMusicManager bkmManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        if(!EndGameChecked && IsEndGame())
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
        EndGameChecked = true;
        return true;
    }

    //back to menu
    public void EndGame()
    {
        // Ensure WinScene is enabled
        switchEffect.UIPanel.SetActive(false);
        cutAway.ActivateWin();
        bkmManager.PlayWinningMusic();
        // click to continue
        StartCoroutine(PlayVideoAndWaitForClick());
    }

    // Coroutine to play video and wait for player input
    private IEnumerator PlayVideoAndWaitForClick()
    {
        // Ensure VideoPlayer is enabled and has time to initialize
        yield return new WaitForEndOfFrame();

        // **Optimized Input Handling**
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        yield return new WaitForSeconds(0.1f);

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
