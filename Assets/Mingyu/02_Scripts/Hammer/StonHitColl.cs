using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonHitColl : HitColider
{
    private void Start()
    {
        base.Start();
        Destroy(this.gameObject, 3f);
    }

    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (owner != Entity.Player)
        {
            if (other.gameObject.name.Contains("APO") &&
                other.gameObject.GetComponent<SkillManager>())
                isAbleDestroy = true;
        }
    }
}
