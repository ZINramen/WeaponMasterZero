using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBoss_BulletHitColl : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {
        // 맞은 대상이 owner, 총알이 맞거나, 설치기가 맞거나, 플레이어의 공격 범위 거나 하면 사라지지 않음
        if(other.gameObject.GetComponent<Entity>() == owner) {}
        else if (other.gameObject.GetComponent<HitColider>()) {}
        else
            this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
    }
}
