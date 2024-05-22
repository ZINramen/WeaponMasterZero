using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBoss_HitAreaCheck : MonoBehaviour
{
    public LastBoss_HandHitAreaPos hitAreaPos;
    public bool isHit_Player;

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
}
