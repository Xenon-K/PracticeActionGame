using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UltEffect : MonoBehaviour
{
    public GameObject phase1;
    public GameObject phase2;
    public GameObject phase3;
    public GameObject phase4;
    public GameObject phase5;

    public void SetPhase(int ult_charge)
    {
        TurnOff();
        if (ult_charge >= 0 && ult_charge < 750)
            phase1.SetActive(true);
        else if (ult_charge >= 750 && ult_charge < 1500)
            phase2.SetActive(true);
        else if (ult_charge >= 1500 && ult_charge < 2250)
            phase3.SetActive(true);
        else if (ult_charge >= 2250 && ult_charge < 3000)
            phase4.SetActive(true);
        else if(ult_charge == 3000)
            phase5.SetActive(true);
    }

    private void TurnOff()
    {
        phase1.SetActive(false);
        phase2.SetActive(false);
        phase3.SetActive(false);
        phase4.SetActive(false);
        phase5.SetActive(false);
    }
}
