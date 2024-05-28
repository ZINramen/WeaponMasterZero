using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Absorb_HitCol : HitColider
{
    [SerializeField] private float deleteTime = 3f;
    private float deleteCount;
    
    public float downSpeed;

    private void Update()
    {
        deleteCount += Time.deltaTime;

        if (deleteCount >= deleteTime)
        {
            owner.gameObject.GetComponent<LastBoss_Ctrl>().Delete_AbsorbHand();
            Destroy(this.gameObject.transform.parent.gameObject);
        }
    }

    private float addHP_Amount;
    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("회복");
            
            float current_BossHP = owner.gameObject.GetComponent<Entity>().GetHp();
            
            if(current_BossHP > 0)
                owner.gameObject.GetComponent<Entity>().SetHp(current_BossHP + addHP_Amount);
        }
    }

    public void SetAddHP(float input_addHP)
    {
        addHP_Amount = input_addHP;
    }
}
