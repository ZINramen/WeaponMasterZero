using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shield_HitCol : HitColider
{
    protected override void EachObj_HitSetting(Collider2D other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.tag == "Camera")
        {
            Debug.Log(other.gameObject.name);
            owner.gameObject.GetComponent<ShieldMon_Ctrl>().EndRush_Setting();
        }
    }
}
