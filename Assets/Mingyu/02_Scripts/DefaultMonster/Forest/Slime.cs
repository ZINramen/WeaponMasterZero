using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Default_Monster
{
    [SerializeField] private float m_dAtt_dist;
    [SerializeField] private float m_traceDist;
    
    void Start()
    {
        base.Start();
        monsterState = new Default_MonsterState();
        Init_StateValueData(ref monsterState);

        stopDelayTime = 1.5f;
    }
    
    protected override void MoveSetting() {}
    
    public void MoveXPos_Slime(float x)
    {
        this.gameObject.GetComponent<Movement>().speed = 2f;
        
        int plus = 1;
        if (transform.localEulerAngles.y == 180) plus = -1;
        myRd.AddForce(new Vector2(x * 100 * plus, plus * 100f), ForceMode2D.Impulse);

        isMoveEnd = true;
    }

    protected override void Init_StateValueData(ref Default_MonsterState state)
    {
        state.defaultAtt_dist = m_dAtt_dist;
        state.traceDistance = m_traceDist;
    }
}
