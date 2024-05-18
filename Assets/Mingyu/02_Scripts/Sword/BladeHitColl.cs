using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeHitColl : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (other.gameObject.name.Contains("Blade"))
            return;

        this.gameObject.GetComponent<Rigidbody2D>().simulated = false;
        this.gameObject.GetComponent<Animator>().SetTrigger("Broken");
    }
}
