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
                && !this.gameObject.GetComponent<BulletCtrl>().Get_IsPlayerParring() )
            {
                if (this.gameObject.GetComponent<BulletCtrl>().bulletType == BulletCtrl.BulletType.General)
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
                if(other.gameObject.GetComponent<Entity>() == owner 
                   || other.gameObject.GetComponent<BulletCtrl>() 
                     || other.gameObject.GetComponent<Entity>() ) {}
                
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

                        float attackPower = this.gameObject.GetComponent<BulletCtrl>().Get_ShootingForce();
                        
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
                
                // owner와 같은 총알이라면
                else if(other.gameObject.GetComponent<Entity>() == owner || other.gameObject.GetComponent<BulletCtrl>() ) {}
            
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
