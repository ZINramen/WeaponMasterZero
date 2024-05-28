using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistHitCol : HitColider
{
    [SerializeField] private float deleteTime = 3f;
    private float deleteCount;

    private void Update()
    {
        deleteCount += Time.deltaTime;

        if (deleteCount >= deleteTime)
        {
            owner.gameObject.GetComponent<LastBoss_Ctrl>().DeleteFist();
            Destroy(this.gameObject);
        }
    }
}
