using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwordHitCol : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<HitColider>() &&
            other.gameObject.GetComponent<HitColider>().attType == HitColider.AttackType.Player_SwordAtt)
        {
            Debug.Log("패링");
            GameObject player = other.gameObject.transform.parent.gameObject;

            if (player != null && player.gameObject.GetComponent<Entity>().GetHp() > 0)
            {
                player.gameObject.GetComponent<StopTime>().DelayTime();
                
                this.transform.parent.GetComponent<Default_Monster>().Check_AttackHitCol(0);
                this.transform.parent.GetComponent<Skeleton>().ParringCheck(0);
                
                this.transform.parent.GetComponent<Animator>().SetTrigger("Stun");
                this.transform.parent.GetComponent<Skeleton>().ParringHit();
                
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
