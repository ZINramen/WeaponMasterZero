using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextExchanger : MonoBehaviour
{
    public Text textBlock;
    public Text textBlockEng;

    void Start()
    {
       if(PlayerPrefs.GetInt("Country_Code", 1) > 0)
       {
            textBlockEng.enabled = true;
            textBlock.enabled = false;
       }
       else
       {
            textBlock.enabled = true;
            textBlockEng.enabled = false;
       }
    }
}
