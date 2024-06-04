using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.TextCore.Text;
using UnityEngine;

public class GMon_BulletHit : HitColider
{
    private float Curr_thrustValue;
    private float Curr_attackForce;
    private float Curr_flyingAttackForce;

    private void Start()
    {
        Destroy(this.gameObject, 4f);
    }

    protected override void EachObj_HitSetting(Collider2D other)
    {
        if ( this.gameObject.GetComponent<BulletCtrl>() )
        {
            if (other.gameObject.GetComponent<HitColider>() &&
                other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_SwordAtt)
            {
                this.gameObject.GetComponent<BulletCtrl>().Parring(other.gameObject);
            }
            
            // 플레이어한테 맞거나, 땅에 맞으면 사라짐
            if(other.gameObject.name == "APO" || other.gameObject.layer == 10)
                this.gameObject.GetComponent<BulletCtrl>().DestoryBullet();
            }
    }
}
