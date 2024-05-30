using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineCtrl : MonoBehaviour
{
    [SerializeField] private GameObject[] actorObjects;
    [SerializeField] private GameObject[] playableObjects;
    
    public void ActivateActors()
    {
        foreach (var obj in actorObjects)
        {
            obj.SetActive(true);
        }

        foreach (var obj in playableObjects)
        {
            obj.SetActive(false);
        }
    }
    
    public void DeactivateActors()
    {
        foreach (var obj in playableObjects)
        {
            obj.SetActive(true);
        }
        
        foreach (var obj in actorObjects)
        {
            obj.SetActive(false);
        }
    }
}
