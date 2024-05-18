using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall_HitColl : HitColider
{
    private int MaxHP = 3;
    [SerializeField] private int HP;
    
    private void Start()
    {
        HP = MaxHP;
    }

    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (other.gameObject.GetComponent<HitColider>()
            && other.gameObject.GetComponent<HitColider>().attType != AttackType.none)
        {
            if (HP > (MaxHP / 3))
            {
                if (other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_FinishdAtt)
                {
                    HP -= MaxHP;
                    this.gameObject.GetComponent<Animator>().SetTrigger("Die");
                    isAbleDestroy = true;
                }
                else
                {
                    HP--;
                }
            }
            else
            {
                isAbleDestroy = true;
            }
        }
    }
    
    protected override void EachObj_DeleteSetting(GameObject deleteObj)
    {
        Destroy(deleteObj.transform.parent.gameObject);
    }
}
