using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSetting : MonoBehaviour
{
    public GameObject[] relevant_Obj;
    public void Change_Lang(int code)
    {
        PlayerPrefs.SetInt("Country_Code", code);
        foreach (var obj in relevant_Obj)
        {
            obj.SetActive(false);
        }
        if (relevant_Obj.Length > 0)
        {
            relevant_Obj[PlayerPrefs.GetInt("Country_Code", 1)].SetActive(true);
        }
    }
    private void Start()
    {
        relevant_Obj[PlayerPrefs.GetInt("Country_Code", 1)].SetActive(true);
    }
}
