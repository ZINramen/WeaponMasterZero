using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class ShieldMon_Ctrl : Default_Monster
{
    [SerializeField] private float m_dAtt_dist;
    [SerializeField] private float m_traceDist;
    [SerializeField] private float rushSpeed;

    public bool NoRush = false;
    public Shield_HitCol hit;

    [SerializeField] private bool isRush = false;
        
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
                EndRush();
            }
        }

        //Debug.Log((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x);

        if (this.gameObject.transform.eulerAngles.y == 180)
        {
            if ((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x < 0)
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.invincible;
            else
            {
                if(!NoRush)
                    this.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
            }
        }
        else
        {
            if ((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x > 0)
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.invincible;
            else
            {
                if (!NoRush)
                    this.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
            }
        }
    }

    public void Rush()
    {
        if (NoRush) return;
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position - transform.right * 2f, -transform.right, 8);
        if (hit2D && hit2D.transform == Entity.Player.transform)
        {
            if (!isNot_ChangeState)
            {
                hit.targetPosX = player.transform.position.x - transform.right.x * 2;
            }

            isNot_ChangeState = true;
            this.gameObject.GetComponent<Animator>().SetBool("isEndRush", false);

            rushSpeed = this.transform.rotation.eulerAngles.y == 180 ? Mathf.Abs(rushSpeed) : -Mathf.Abs(rushSpeed);
            Move(0, this.transform.position.x > player_pos.x ? 1 : -1);

            isRush = true;
        }
    }

    public void EndRush()
    {
        isNot_ChangeState = false;
        
        Move(0, nextMove > 0 ? 1 : -1);
        this.gameObject.GetComponent<Animator>().SetBool("isEndRush", true);
        
        EndAttack();
    }
}
