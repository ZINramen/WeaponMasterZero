using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DontGo : MonoBehaviour
{
    [SerializeField] private GameObject[] MonsterList;
    [SerializeField] private GameObject DontGo_Obj;

    private int maxMonsterCount;
    private int dieMonster;

    private void Start()
    {
        maxMonsterCount = MonsterList.Length;
    }

    public void Update()
    {
        foreach (GameObject monster in MonsterList)
        {
            if (monster == null)
            {
                dieMonster++;

                if (dieMonster >= maxMonsterCount)
                {
                    Destroy(DontGo_Obj);
                }
            }
            else
            {
                dieMonster = 0;
            }
        }
    }
}
