using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall_HitColl : HitColider
{
    private int MaxHP = 3;
    [SerializeField] private int HP;
    
    public float moveSpeed;
    private float rollingAngle;
    private float rollingPower = -50f;
    
    private void Start()
    {
        HP = MaxHP;
    }

    private void Update()
    {
        if (moveSpeed > 0)
            rollingPower = -Mathf.Abs(rollingPower);
        else
            rollingPower = Mathf.Abs(rollingPower);
        
        this.gameObject.transform.parent.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);

        rollingAngle += Time.deltaTime * rollingPower; 
        this.gameObject.transform.rotation = Quaternion.Euler(0, 0, rollingAngle);
    }

    protected override void EachObj_HitSetting(Collider2D other)
    {
        
        if (other.gameObject.GetComponent<HitColider>())
        {
            HP--;
            
            if (HP > 0)
            {
                if (other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_FinishdAtt)
                {
                    HP -= MaxHP;
                    isAbleDestroy = true;
                }
            }
            else
            {
                if (DestroyEffect)
                {
                    Instantiate(DestroyEffect, transform.position, Quaternion.identity);
                }
                EachObj_DeleteSetting(this.gameObject);
            }
        }
    }
    
    protected override void EachObj_DeleteSetting(GameObject deleteObj)
    {
        Debug.Log("삭제");
        
        owner.gameObject.GetComponent<HammerBoss>().Destroy_SnowBall();
        Destroy(deleteObj.transform.parent.gameObject);
    }
}
