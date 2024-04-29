using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoticonController : MonoBehaviour
{
    Animator a;
    public bool blockControl = false;

    private void Awake()
    {
        a = GetComponent<Animator>();
    }
    private void Update()
    {
        if(blockControl)
            a.SetInteger("FValue", 0);
    }
    public void SetValue(int value) 
    {
        if(!blockControl)
            a.SetInteger("FValue", value);
    }
}
