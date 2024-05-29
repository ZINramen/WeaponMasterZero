using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitColider : MonoBehaviour
{
    public bool telp = false;
    public bool stunTarget = false;
    public float flyingAttackForce = 0;
    public float attackForce = 10;
    public float thrustValue = 0.5f;
    public Entity owner;

    public bool oneHit = false;
    public bool playerIsOwn = false;

    public enum AttackType
    {
        none,
        Player_SwordAtt,
        Player_GunAtt,
        Player_FinishdAtt
    }
    public AttackType attType = AttackType.none;
    public bool isAbleDestroy = false;
    
    public GameObject DestroyEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (owner == other.gameObject.GetComponent<Entity>())
            return;
        if (other.CompareTag("Camera"))
            return;
        if (playerIsOwn && (other.CompareTag("Player")))
            return;
        
        EachObj_HitSetting(other);
        
        if (isAbleDestroy && (other.CompareTag("Enemy") || other.CompareTag("Untagged")))
        {
            if (DestroyEffect)
            {
               Instantiate(DestroyEffect, transform.position, Quaternion.identity);
            }
            EachObj_DeleteSetting(this.gameObject);
        }
        Entity entity = other.GetComponent<Entity>();

        // 보스에게 피격할 시, 날라가는 힘을 없앰
        if (other.gameObject.tag == "Boss" && other.gameObject.GetComponent<Entity>().GetHp() > 0)
        {
            thrustValue = 0;
            flyingAttackForce = 0;

            if (owner && owner != entity && other.gameObject.GetComponent<Boss>())
            {
                other.gameObject.GetComponent<Boss>().HitEffect();
                Debug.Log("맞는 거잖아~");
            }
        }

        if (entity && entity.GetHp() > 0)
        {
            if (owner)
            {
                owner.SetMp(owner.GetMp()+5);
                if ((owner.tag == "Player" && entity.tag != "Player") || (owner.tag != "Player" && entity.tag == "Player"))
                {
                    if (telp)
                    {
                        owner.transform.position = entity.transform.position + (owner.transform.right * 0.01f);
                        if (owner.transform.localEulerAngles.y != 0)
                            owner.transform.localEulerAngles = new Vector3(0, 0, 0);
                        else
                            owner.transform.localEulerAngles = new Vector3(0, -180, 0);
                        Destroy(gameObject);
                    }
                    else
                    {
                        entity.stun = stunTarget;
                        entity.flyingDamagedPower = flyingAttackForce;
                        
                        if (owner.transform.position.x > entity.transform.position.x)
                        {
                            if (!owner || owner.movement.PlayerType || entity.movement.PlayerType)
                                entity.Damaged(attackForce, (-attackForce) * thrustValue);
                        }
                        else
                        {
                            if (!owner || owner.movement.PlayerType || entity.movement.PlayerType)
                                entity.Damaged(attackForce, attackForce * thrustValue);
                        }
                    }
                }
            }
            else
            {
                entity.stun = stunTarget;
                entity.flyingDamagedPower = flyingAttackForce;
                entity.Damaged(attackForce, (-attackForce) * thrustValue);
            }

            if (oneHit == true)
                Destroy(this);
        }
    }

    protected virtual void EachObj_HitSetting(Collider2D other)
    {
    }
    
    protected virtual void EachObj_DeleteSetting(GameObject deleteObj)
    {
        Destroy(gameObject);
    }
}