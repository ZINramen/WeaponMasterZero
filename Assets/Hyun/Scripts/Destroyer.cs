using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float delayTime = 0;
    void Update()
    {
        Destroy(gameObject, delayTime);
    }
}
