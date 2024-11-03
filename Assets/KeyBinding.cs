using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class KeyBinding : MonoBehaviour
{
    public Text[] keys;

    Array keyValues = Enum.GetValues(typeof(KeyCode));

    bool isStart = false;
    int currentKey = -1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            foreach (KeyCode key in keyValues)
            {
                if (Input.GetKeyDown(key))
                {
                    PlayerPrefs.SetInt("key" + currentKey.ToString(), (int)key);
                    currentKey = -1;
                    isStart = false;
                }
            }
        }
        else 
        {
            keys[0].text = ((KeyCode)PlayerPrefs.GetInt("key1", (int)KeyCode.Q)).ToString();
            keys[1].text = ((KeyCode)PlayerPrefs.GetInt("key2", (int)KeyCode.E)).ToString();
            keys[2].text = ((KeyCode)PlayerPrefs.GetInt("key3", (int)KeyCode.W)).ToString();
            keys[3].text = ((KeyCode)PlayerPrefs.GetInt("key4", (int)KeyCode.LeftShift)).ToString();
        }
    }

    public void BindingNewKey(int keyNumber)
    {
        if (currentKey == -1)
        {
            keys[keyNumber - 1].text = "Press Any Key";
            currentKey = keyNumber;
            isStart = true;
        }
    }

    public void DeleteAllKeys()
    {
        PlayerPrefs.DeleteKey("key1");
        PlayerPrefs.DeleteKey("key2");
        PlayerPrefs.DeleteKey("key3");
        PlayerPrefs.DeleteKey("key4");
    }
}
