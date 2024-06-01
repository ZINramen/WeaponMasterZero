using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShooting_HitCol : HitColider
{
    private void Start()
    {
        Destroy(this.gameObject, 
            this.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().duration + 0.5f);
    }

    protected override void EachObj_HitSetting(Collider2D other)
    {
        if ( (other.gameObject.tag == "Player" || other.gameObject.layer == 10)
            && other.gameObject.GetComponent<HitColider>() == null )
        {
            isAbleDestroy = true;
            
            if (DestroyEffect)
            {
                Instantiate(DestroyEffect, transform.position, Quaternion.identity);
            }
            
            EachObj_DeleteSetting(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
    
    protected override void EachObj_DeleteSetting(GameObject deleteObj)
    {
        this.gameObject.GetComponent<Rigidbody2D>().simulated = false;
        Destroy(deleteObj, DestroyEffect.GetComponent<ParticleSystem>().duration);
    }
}
