using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public bool fromParent;
    public float delayTime = 0;
    void Update()
    {
        if(fromParent)
        {
            transform.parent = null;
        }
        else
            Destroy(gameObject, delayTime);
    }
}
