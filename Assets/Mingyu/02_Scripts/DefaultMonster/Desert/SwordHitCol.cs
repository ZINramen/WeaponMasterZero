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
                player.gameObject.GetComponent<StopTime>().DelayTime();
                
                owner.gameObject.GetComponent<Animator>().SetTrigger("Stun");
                owner.gameObject.GetComponent<Skeleton>().ParringHit();
                owner.gameObject.GetComponent<Default_Monster>().Check_AttackHitCol(0);
                
                StartCoroutine("returnTimeDelay", player);
            }
        }
    }
    
    private IEnumerator returnTimeDelay(GameObject m_player)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        m_player.gameObject.GetComponent<StopTime>().PlayALLTime();
    }
}
