using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_HitCollder : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {
        // 일반 탄일 경우
        if ( this.gameObject.GetComponent<BulletCtrl>() && 
             this.gameObject.GetComponent<BulletCtrl>().Get_BulletType() == BulletCtrl.BulletType.General )
        {
            // 만약 플레이어가 공격중이라면
            if (other.gameObject.GetComponent<HitColider>() &&
                other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_SwordAtt)
            {
                this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
            }
        }
    }
}
