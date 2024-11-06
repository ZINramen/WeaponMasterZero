using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class ShieldMon_Ctrl : MonoBehaviour
{
    private GameObject player;
    private bool isExplosionHit = false;
    
    private void Start()
    {
        GameObject[] playerTagObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerTagObj in playerTagObjects)
        {
            if (playerTagObj.name == "APO")
                player = playerTagObj;
        }
    }

    private void Update()
    {
        if (player)
        {
            if (this.gameObject.transform.eulerAngles.y == 180)
            {
                if ((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x < 0.3f && !isExplosionHit)
                    this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
                else 
                    this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
            }
            else
            {
                if ((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x > -0.3f && !isExplosionHit)
                    this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
                else 
                    this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HitColider hit = other.GetComponent<HitColider>();
        if (hit)
        {
            if (hit.attType == HitColider.AttackType.Player_FinishdAtt)
                GetComponent<Entity>().SetHp(0);
        }
        if (other.gameObject.name.Contains("Explosion"))
        {
            if (this.gameObject.transform.eulerAngles.y == 180)
            {
                if ((this.gameObject.transform.position - other.gameObject.transform.position).normalized.x > 0.3f)
                {
                    isExplosionHit = true;
                    this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
                    this.gameObject.GetComponent<Entity>().Damaged(other.gameObject.GetComponent<HitColider>().attackForce);
                }
            }
                
            else
            {
                if ((this.gameObject.transform.position - other.gameObject.transform.position).normalized.x < -0.3f)
                {
                    isExplosionHit = true;
                    this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
                    this.gameObject.GetComponent<Entity>().Damaged(other.gameObject.GetComponent<HitColider>().attackForce);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("Explosion"))
        {
            isExplosionHit = false;
        }
    }
}
