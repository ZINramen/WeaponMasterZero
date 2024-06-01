using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RotateFix : MonoBehaviour
{
    public bool Left = false;
    void Update()
    {
        if (Left)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
