using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Mingyu : Default_Monster
{    
    [SerializeField] private float m_dAtt_dist;
    [SerializeField] private float m_traceDist;
    
    void Start()
    {
        base.Start();
        monsterState = new Default_MonsterState();
        Init_StateValueData(ref monsterState);

        stopDelayTime = 1.5f;

        AttHitCol = this.gameObject.transform.GetChild(0).gameObject;
        AttHitCol.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    protected override void Init_StateValueData(ref Default_MonsterState state)
    {
        state.defaultAtt_dist = m_dAtt_dist;
        state.traceDistance = m_traceDist;
    }
}
