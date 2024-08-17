using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Title_Prologue", LoadSceneMode.Single);
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.SetInt("cur_chapter", 4);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
