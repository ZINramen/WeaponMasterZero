using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clim_Ctrl : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.layer == 10)
        {
            if (DestroyEffect)
            {
                GameObject dummyEffect = Instantiate(DestroyEffect, transform.position, Quaternion.identity);
                this.gameObject.GetComponent<Rigidbody2D>().simulated = false;
                Destroy(this.gameObject, dummyEffect.gameObject.GetComponent<ParticleSystem>().duration);
            }
        }
    }
}
