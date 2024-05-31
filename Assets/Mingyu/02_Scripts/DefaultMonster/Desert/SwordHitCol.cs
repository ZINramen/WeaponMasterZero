using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwordHitCol : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (other.gameObject.GetComponent<HitColider>() &&
            other.gameObject.GetComponent<HitColider>().attType == AttackType.Player_SwordAtt)
        {
            Debug.Log("패링");
            
            GameObject player = other.gameObject.transform.parent.gameObject;

            if (player != null && player.gameObject.GetComponent<Entity>().GetHp() > 0)
            {
                owner.gameObject.GetComponent<Animator>().SetTrigger("Stun");
                owner.gameObject.GetComponent<Skeleton>().ParringHit();

                owner.gameObject.GetComponent<Skeleton>().EndAttack();
                owner.gameObject.GetComponent<Default_Monster>().Check_AttackHitCol(0);

                float attackDamage = other.gameObject.GetComponent<HitColider>().attackForce;
                owner.gameObject.GetComponent<Entity>().Damaged(attackDamage * 2);
            }
        }
    }
}
