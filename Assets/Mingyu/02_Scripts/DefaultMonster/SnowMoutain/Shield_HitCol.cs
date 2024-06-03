using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shield_HitCol : HitColider
{
    ShieldMon_Ctrl smc;
    public float targetPosX;

    private void Start()
    {
        smc = owner.GetComponent<ShieldMon_Ctrl>();
    }

    public void Update()
    {
        if (Mathf.Abs(owner.transform.position.x - targetPosX) < 1f)
        {
            owner.GetComponent<ShieldMon_Ctrl>().EndRush();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if(collision.gameObject.layer == 10)
        {
            smc.EndRush();
            owner.transform.rotation = Quaternion.Euler(0, owner.transform.localEulerAngles.x == 0 ? 180 : 0, 0);
        }
    }
}
