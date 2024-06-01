using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class ShieldMon_Ctrl : Default_Monster
{
    [SerializeField] private float m_dAtt_dist;
    [SerializeField] private float m_traceDist;
    [SerializeField] private float rushSpeed;
    
    [SerializeField] private GameObject Ground_Pref;
    private GameObject dummy_GroundObj;

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

        Debug.Log((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x);

        if (this.gameObject.transform.eulerAngles.y == 180)
        {
            if ((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x < 0)
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.invincible;
            else
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
        }
        else
        {
            if ((this.gameObject.transform.position - player.gameObject.transform.position).normalized.x > 0)
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.invincible;
            else
                this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
        }
    }

    public void Rush()
    {
        float stopXPos;

        if (!isNot_ChangeState)
        {
            float angle = this.transform.position.x > player_pos.x ? 0 : 180;
            this.transform.rotation = Quaternion.Euler(0, angle, 0);

            if (angle == 180)
                stopXPos = 1.5f;
            else
                stopXPos = 1.5f;

            Vector3 sponPos = new Vector3(player_pos.x + stopXPos, this.transform.position.y - 0.5f, 0);
            dummy_GroundObj = GameObject.Instantiate(Ground_Pref, sponPos, Quaternion.identity);
        }

        isNot_ChangeState = true;
        this.gameObject.GetComponent<Animator>().SetBool("isEndRush", false);
        
        rushSpeed = this.transform.rotation.eulerAngles.y == 180 ? Mathf.Abs(rushSpeed) : -Mathf.Abs(rushSpeed);
        Move(0, this.transform.position.x > player_pos.x ? 1 : -1);
            
        isRush = true;
    }

    public void Destory_StopPos()
    {
        Destroy(dummy_GroundObj);
    }

    public void EndRush_Setting()
    {
        Invoke("EndRush", 1f);
    }

    private void EndRush()
    {
        isNot_ChangeState = false;
        
        Move(0, nextMove > 0 ? 1 : -1);
        this.gameObject.GetComponent<Animator>().SetBool("isEndRush", true);
        isRush = false;
        
        EndAttack();
    }
}
