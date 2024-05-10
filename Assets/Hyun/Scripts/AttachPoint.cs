using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPoint : MonoBehaviour
{
    public bool noCatch = false;
    public Entity owner;
    public Entity target;

    public bool Ult = false;
    public float UltDamage = 0;

    private void Start()
    {
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        Entity entity = other.GetComponent<Entity>();
        if(!noCatch)
        if (entity && target == null)
            if (entity != owner && entity.DamageBlock != Entity.DefenseStatus.invincible)
            {
                target = entity;
                target.DamageBlock = Entity.DefenseStatus.invincible;
                
            }
    }
    private void Update()
    {
        if (target)
        {
            target.Damaged(0, 0);
            target.movement.SetVelocityZero();
            target.transform.parent = transform;
            target.movement.Freeze();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity)
            if (entity == target)
            {
                if (UltDamage > 0)
                {
                    target.DamageBlock = Entity.DefenseStatus.Nope;
                    target.waitTime = 0;
                    if (owner.transform.localEulerAngles.y != 0)
                    {
                        target.Damaged(UltDamage, -10);
                        target.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        target.Damaged(UltDamage, 10);
                        owner.ResetMp();
                    }

                }
                else
                {
                    entity.SetHp(entity.GetHp() - 5.0f);
                }
                target = null;
                entity.movement.UnFreeze();
                entity.transform.parent = null; 
                

                if (Ult) 
                {
                    owner.aManager.ani.SetTrigger("Start");
                }
            } 
    }
}
