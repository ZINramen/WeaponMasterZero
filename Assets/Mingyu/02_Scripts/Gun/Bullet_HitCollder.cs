using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.TextCore.Text;
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
            // 플레이어가 패링하지 않는 탄이 설치기에 맞으면, 그냥 지나감
            if (other.gameObject.GetComponent<Install_Ctrl>() )
            {
                if(this.gameObject.GetComponent<BulletCtrl>().bulletType == BulletCtrl.BulletType.Parring &&
                   !this.gameObject.GetComponent<BulletCtrl>().Get_IsPlayerParring() )
                    this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
                
                if (!this.gameObject.GetComponent<BulletCtrl>().Get_IsPlayerParring())
                {
                    Curr_thrustValue = thrustValue;
                    Curr_attackForce = attackForce;
                    Curr_flyingAttackForce = flyingAttackForce;

                    thrustValue = 0;
                    attackForce = 0;
                    flyingAttackForce = 0;
                }
            }
            
            // 일반 탄일 경우
            if (this.gameObject.GetComponent<BulletCtrl>().bulletType == BulletCtrl.BulletType.General)
            {
                // 맞은 대상이 owner, 총알이 맞거나, 설치기가 맞거나, 플레이어의 공격 범위 거나 하면 사라지지 않음
                if(other.gameObject.GetComponent<Entity>() == owner) {}
                else if(other.gameObject.GetComponent<Install_Ctrl>()) {}
                else if (other.gameObject.GetComponent<HitColider>()) {}
                else
                    this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
            }
            
            // 패링 탄일 경우
            else if (this.gameObject.GetComponent<BulletCtrl>().bulletType == BulletCtrl.BulletType.Parring)
            {
                if (other.gameObject.GetComponent<HitColider>() &&
                    other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_SwordAtt)
                {
                    this.gameObject.GetComponent<BulletCtrl>().Parring(other.gameObject);
                    this.gameObject.GetComponent<BulletCtrl>().wallParringHP -= 2;
                }
                // 만약 벽이면 n번 튕김
                if (other.gameObject.GetComponent<WallCtrl>())
                {
                    if (this.gameObject.GetComponent<BulletCtrl>().wallParringHP > 0)
                    {
                        this.gameObject.GetComponent<BulletCtrl>().StopMove();

                        float attackPower = this.gameObject.GetComponent<BulletCtrl>().addForce;
                        
                        Vector2 incomingVector = this.gameObject.transform.position - other.transform.position;
                        Vector2 normalVector = other.gameObject.GetComponent<WallCtrl>().Direction;

                        float angle = Vector2.Angle(incomingVector, normalVector);
                        
                        float reflectAngle_z;

                        if (other.gameObject.GetComponent<WallCtrl>().wallType != WallCtrl.WallType.Up)
                            reflectAngle_z = 180 - Mathf.Abs(angle);
                        else
                            reflectAngle_z = -this.gameObject.GetComponent<BulletCtrl>().install_ZValue;
                        
                        Vector3 ReflectAngle = new Vector3(0, 0, 0);
                        
                        ReflectAngle.z = reflectAngle_z;
                        this.gameObject.GetComponent<BulletCtrl>().ShootingBullet( attackPower, Quaternion.Euler(ReflectAngle) );
                        this.gameObject.GetComponent<BulletCtrl>().wallParringHP--;
                    }
                    else
                    {
                        this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
                    }
                }
                
                // 맞은 대상이 owner, 총알이 맞거나
                else if(other.gameObject.GetComponent<Entity>() == owner || other.gameObject.GetComponent<BulletCtrl>() ) {}
            
                else if(this.gameObject.GetComponent<BulletCtrl>().Get_IsPlayerParring()) {}
                
                else
                    this.gameObject.GetComponent<BulletCtrl>().BrokenBullet();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Install_Ctrl>())
        {
            thrustValue = Curr_thrustValue;
            attackForce = Curr_attackForce;
            flyingAttackForce = Curr_flyingAttackForce;
        }
    }
}
