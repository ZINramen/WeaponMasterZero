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
                
                if (this.transform.parent.GetComponent<Skeleton>())
                {
                    this.transform.parent.GetComponent<Skeleton>().ParringCheck(0);
                    this.transform.parent.GetComponent<Skeleton>().ParringHit();
                }
                else if (this.transform.parent.GetComponent<Zombie_Mingyu>())
                {
                    this.transform.parent.GetComponent<Zombie_Mingyu>().ParringCheck(0);
                    this.transform.parent.GetComponent<Zombie_Mingyu>().ParringHit();
                }
                
                this.transform.parent.GetComponent<Animator>().SetTrigger("Stun");
                
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
