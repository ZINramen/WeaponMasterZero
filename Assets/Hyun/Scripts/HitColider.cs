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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Boss")
        {
            thrustValue = 0;
            flyingAttackForce = 0;
        }
        
        Entity entity = other.GetComponent<Entity>();
        if(entity)
        if(entity != owner)
        {
            if (telp) 
            {
                owner.transform.position = entity.transform.position + (owner.transform.right*0.01f);
                if(owner.transform.localEulerAngles.y != 0) 
                    owner.transform.localEulerAngles = new Vector3(0,0,0);
                else
                    owner.transform.localEulerAngles = new Vector3(0,-180,0);
                    Destroy(gameObject);
            }
            else
            {
                entity.stun = stunTarget;
                entity.flyingDamagedPower = flyingAttackForce;
                if (owner && owner.transform.localEulerAngles.y == 180)
                {
                    if(!owner || owner.movement.PlayerType || entity.movement.PlayerType)
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
}
