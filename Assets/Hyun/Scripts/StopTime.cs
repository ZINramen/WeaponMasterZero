using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTime : MonoBehaviour
{
    public void StopALLTime() 
    {
        Time.timeScale = 0;
    }

    public void PlayALLTime()
    {
        Time.timeScale = 1;
    }
}
