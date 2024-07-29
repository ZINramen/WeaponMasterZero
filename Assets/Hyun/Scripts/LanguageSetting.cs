using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class LanguageSetting : MonoBehaviour
{
    public GameObject[] relevant_Obj;
    public static int country_Code;
    public void Change_Lang(int code)
    {
        country_Code = code;
        PlayerPrefs.SetInt("Country_Code", code);
        foreach (var obj in relevant_Obj)
        {
            obj.SetActive(false);
        }
        if (relevant_Obj.Length > 0)
        {
            relevant_Obj[country_Code].SetActive(true);
        }
    }
    private void Start()
    {
        country_Code = PlayerPrefs.GetInt("Country_Code");
        relevant_Obj[country_Code].SetActive(true);
    }
}
