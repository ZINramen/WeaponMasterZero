using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonHitColl : HitColider
{
    private void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (other != owner && !other.gameObject.name.Contains("IceRain"))
            isAbleDestroy = true;
    }
}
