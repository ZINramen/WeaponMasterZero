using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class StopTime : MonoBehaviour
{
    [Range(0f, 1f)] public float timeScale;
    
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
