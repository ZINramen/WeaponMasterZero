using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_HitCollder : HitColider
{
    private float Curr_thrustValue;
    private float Curr_attackForce;
    private float Curr_flyingAttackForce;
    
    protected override void EachObj_HitSetting(Collider2D other)
    {
        if ( this.gameObject.GetComponent<BulletCtrl>() )
        {
            // 플레이어가 패링하지 않는 탄이 설치기에 맞으면, 그냥 튕겨져 나감 
            if (other.gameObject.GetComponent<Install_Ctrl>() 
                && !this.gameObject.GetComponent<BulletCtrl>().Get_IsPlayerParring())
            {
                Curr_thrustValue = thrustValue;
                Curr_attackForce = attackForce;
                Curr_flyingAttackForce = flyingAttackForce;
                    
                thrustValue = 0;
                attackForce = 0;
                flyingAttackForce = 0;
            }
            
            // 일반 탄일 경우
            if (this.gameObject.GetComponent<BulletCtrl>().bulletType == BulletCtrl.BulletType.General)
            {
                // 만약 플레이어가 공격중이라면
                if (other.gameObject.GetComponent<HitColider>() &&
                    other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_SwordAtt)
                {
                    this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
                }
            }
            
            // 패링 탄일 경우
            else if (this.gameObject.GetComponent<BulletCtrl>().bulletType == BulletCtrl.BulletType.Parring)
            {
                if (other.gameObject.GetComponent<HitColider>() &&
                    other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_SwordAtt)
                {
                    this.gameObject.GetComponent<BulletCtrl>().Parring(other.gameObject);
                    this.gameObject.GetComponent<BulletCtrl>().wallParringHP--;
                }

                if (other.gameObject.tag == "Wall")
                {
                    if (this.gameObject.GetComponent<BulletCtrl>().wallParringHP > 0)
                    {
                        this.gameObject.GetComponent<BulletCtrl>().StopMove();

                        float attackPower = this.gameObject.GetComponent<BulletCtrl>().Get_ShootingForce();
                        Vector2 attDir =  other.gameObject.transform.position - this.gameObject.transform.position;
                        this.gameObject.GetComponent<BulletCtrl>().ShootingBullet(attackPower, Quaternion.Euler(-attDir));
                    }
                    else
                    {
                        this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Install_Ctrl>())
        {
            Curr_thrustValue = thrustValue;
            Curr_attackForce = attackForce;
            Curr_flyingAttackForce = flyingAttackForce;
        }
    }
}
