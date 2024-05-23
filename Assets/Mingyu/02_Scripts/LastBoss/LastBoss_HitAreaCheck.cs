using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBoss_HitAreaCheck : MonoBehaviour
{
    public LastBoss_HandHitAreaPos hitAreaPos;
    public bool isHit_Player;

    [SerializeField] private GameObject BossHand;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            isHit_Player = true;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            isHit_Player = false;
    }

    public void HandAttack()
    {
        if(hitAreaPos == LastBoss_HandHitAreaPos.UpArea)
            BossHand.gameObject.GetComponent<Animator>().SetInteger("isAttackType", (int)LastBoss_HandHitAreaPos.UpArea);
        
        else if(hitAreaPos == LastBoss_HandHitAreaPos.RightArea)
            BossHand.gameObject.GetComponent<Animator>().SetInteger("isAttackType", (int)LastBoss_HandHitAreaPos.RightArea);
        
        else if(hitAreaPos == LastBoss_HandHitAreaPos.DownArea)
            BossHand.gameObject.GetComponent<Animator>().SetInteger("isAttackType", (int)LastBoss_HandHitAreaPos.DownArea);
        
        else if(hitAreaPos == LastBoss_HandHitAreaPos.LeftArea)
            BossHand.gameObject.GetComponent<Animator>().SetInteger("isAttackType", (int)LastBoss_HandHitAreaPos.LeftArea);
    }
}
