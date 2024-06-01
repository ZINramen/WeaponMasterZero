using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputDetection : MonoBehaviour
{
    public bool any_key = false;
    public KeyCode input_key;
    public UnityEvent input_action;

    // Update is called once per frame
    void Update()
    {
        if (any_key)
        {
            if (Input.anyKeyDown)
            {
                input_action.Invoke();
            }
        }
        else if (Input.GetKeyDown(input_key))
        {
            input_action.Invoke();
        }   
    }
}
