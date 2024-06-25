using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameSettings : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        Application.targetFrameRate = 60;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
