using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMon_Ctrl : Default_Monster
{
    [SerializeField] private float m_dAtt_dist;
    [SerializeField] private float m_traceDist;
    [SerializeField] private float rushSpeed;

    private bool isRush;
        
    void Start()
    {
        base.Start();
        monsterState = new Default_MonsterState();
        Init_StateValueData(ref monsterState);
    
        stopDelayTime = 1.5f;
        groundApproachDist = 0.5f;
    
        AttHitCol = this.gameObject.transform.GetChild(0).gameObject;
        AttHitCol.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
    
    protected override void Init_StateValueData(ref Default_MonsterState state)
    {
        state.defaultAtt_dist = m_dAtt_dist;
        state.traceDistance = m_traceDist;
    }

    protected override void UpdateSetting()
    {
        if (isRush)
        {
            if (rayHit.collider != null)
            {
                Move(rushSpeed, rushSpeed > 0 ? 1 : -1);
            }
            else
            {
                EndRush_Setting();
            }
        }
    }

    public void Rush()
    {
        this.gameObject.GetComponent<Animator>().SetBool("isEndRush", false);
        
        this.transform.rotation = Quaternion.Euler(0, this.transform.position.x > player_pos.x ? 0 : 180, 0);
        this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.invincible;

        rushSpeed = this.transform.rotation.eulerAngles.y == 180 ? Mathf.Abs(rushSpeed) : -Mathf.Abs(rushSpeed);
        Move(0, this.transform.position.x > player_pos.x ? 1 : -1);
            
        isRush = true;
    }

    public void EndRush_Setting()
    {
        Move(0, nextMove > 0 ? 1 : -1);
        this.gameObject.GetComponent<Animator>().SetBool("isEndRush", true);
        isRush = false;
        
        EndAttack();
    }
}
