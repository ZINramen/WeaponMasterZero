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
            
            // 맞은 대상이 owner, 총알이 맞거나
            if(other.gameObject.GetComponent<Entity>() == owner || other.gameObject.GetComponent<BulletCtrl>() ) {}
        
            else if(this.gameObject.GetComponent<BulletCtrl>().Get_IsPlayerParring()) {}
            
            else
                this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
            
            }
    }
}
