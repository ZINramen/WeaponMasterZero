using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class StopTime : MonoBehaviour
{
    public bool inputCanPlayTime = false;
    public KeyCode keyc;

    [Range(0f, 1f)] public float timeScale;

    private void Update()
    {
        if (inputCanPlayTime)
        {
            if (Input.GetKeyDown(keyc))
            {
                PlayALLTime();
            }
        }
    }

    public void StopALLTime() 
    {
        Time.timeScale = 0;
    }

    public void PlayALLTime()
    {
        Time.timeScale = 1;
    }

    public void DelayTime()
    {
        Time.timeScale = timeScale;
    }
}
