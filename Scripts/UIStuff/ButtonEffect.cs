using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour
{
    public Image baseButton;
    public Image normalLayer;
    public Image energyLayer;
    public Text TimerText;

    public void PressedButton()
    {
        baseButton.gameObject.SetActive(true);
        normalLayer.gameObject.SetActive(false);
        energyLayer.gameObject.SetActive(false);
    }

    public void NormalState()
    {
        baseButton.gameObject.SetActive(false);
        normalLayer.gameObject.SetActive(true);
        energyLayer.gameObject.SetActive(false);
    }

    public void FullEnergyState()
    {
        baseButton.gameObject.SetActive(false);
        normalLayer.gameObject.SetActive(false);
        energyLayer.gameObject.SetActive(true);
    }

    public void EvadeCoolDownState(float currentCoolDown)
    {
        baseButton.gameObject.SetActive(false);
        normalLayer.gameObject.SetActive(false);
        energyLayer.gameObject.SetActive(true);
        if(currentCoolDown > 0) 
        {
            TimerText.text = $"{currentCoolDown}";
        }
        else
        {
            TimerText.text = "";
        }

    }
}
