using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBoss : Boss
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Gun;
    }

    protected override void Init_StateValueData(ref Boss_State state)
    {
        
    }
}
