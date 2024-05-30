using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clim_Ctrl : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {   
        if (other.gameObject.tag == "Player")
            return;
    }
}
