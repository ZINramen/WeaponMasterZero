using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Boss_State
{
    public enum State 
    { 
        trace,
        idle = 0,
        DefaultAtt = 1,
        p1_Skill1 = 2,
        p1_Skill2 = 3,
        p2_Skill1 = 4,
        p2_Skill2 = 5,
        p2_Skill3 = 6 
    }
    public State currentState;

    public float traceDistance = 5000f;
    public float defaultAtt_dist;

    public float skill_CoolTime;

    public float p1_Skill1_dist;
    public float p1_Skill2_dist;
    
    public float p2_Skill1_dist;
    public float p2_Skill2_dist;
    public float p2_Skill3_dist;

    public bool isAttacking = false;
    public bool isSkillReady = false;
    public float skill_CountTime = 0f;

    public bool isStopTurn = false;
}

public enum BossType { Sword, Gun, Hammer, Last }

public abstract class Boss : MonoBehaviour
{
    protected GameObject player;
    protected Vector2 player_pos;

    public float move_Speed;
    protected Boss_State bossState;

    public Boss_State Get_CurrBossState()
    {
        return bossState;
    }
    
    protected float bossHP_per = 1.0f;
    protected float distFrom_Player;
    
    // 처음 시작할 땐, 스킬이 활성화 되어있지 않음
    protected Boss_State.State sBossSkill;
    protected int iBossSkill;
    
    protected float skillDist;
    
    #region 충돌 박스 변수 (외부 ref)
    [SerializeField] protected GameObject[] DA_HitArea;
    [SerializeField] protected GameObject[] P1Skill1_HitArea;
    [SerializeField] protected GameObject[] P1Skill2_HitArea;
    [SerializeField] protected GameObject[] P2Skill1_HitArea;
    [SerializeField] protected GameObject[] P2Skill2_HitArea;
    [SerializeField] protected GameObject[] P2Skill3_HitArea;
    private int index = 0;
    #endregion
    
    protected Animator animCtrl;

    protected Material origin_Mat;
    [SerializeField] protected Material hit_Mat;

    public BossType bossType;
    public bool isDie;
    
    // Start is ca
    // lled before the first frame update
    protected void Start()
    {
        player = GameObject.FindWithTag("Player").gameObject;
        origin_Mat = this.gameObject.GetComponent<SpriteRenderer>().material;
        
        if (player != null)
        {
            player_pos = player.GetComponent<Transform>().position;
        }

        animCtrl = GetComponent<Animator>();
    }

    protected abstract void Init_StateValueData(ref Boss_State state);

    // Update is called once per frame
    protected void Update()
    {
        if (this.gameObject.GetComponent<Entity>().GetHp() > 0)
        {
            UpdateState();
            UpdateAnimation();
            UpdateSkillCooldown();
            EachBoss_UpdateSetting();
        }
    }

    private void UpdateState()
    {
        player_pos = player.GetComponent<Transform>().position;
        distFrom_Player = Vector2.Distance(player_pos, transform.position);
        
        // 상황에 따른 동작 구현 FSM
        if (distFrom_Player >= bossState.traceDistance)
        {
            bossState.currentState = Boss_State.State.idle;
        }
        
        // 스킬 준비가 되어있고, 보스가 공격중이 아니라면.
        else if (bossState.isSkillReady && !bossState.isAttacking)
        {
            // 보스 체력에 따라, 스킬이 나올것이 달라짐
            bossHP_per = ( this.GetComponent<Entity>().GetHp() ) / ( this.GetComponent<Entity>().maxHP );
            if (bossHP_per >= 0.5f)
                iBossSkill = Random.Range((int)Boss_State.State.p1_Skill1, (int)Boss_State.State.p1_Skill2 + 1);
            else
                iBossSkill = Random.Range((int)Boss_State.State.p2_Skill1, (int)Boss_State.State.p2_Skill3 + 1);
            iBossSkill = 2;
            
            sBossSkill = Change_IntToState(iBossSkill, ref skillDist);
            
            if (distFrom_Player >= skillDist)
            {
                bossState.currentState = Boss_State.State.trace;
            }
            else
            {
                bossState.currentState = sBossSkill;
                bossState.isAttacking = true;
                bossState.isSkillReady = false;
            }
        }
        
        // 스킬 쿨타임 중일때
        else if (!bossState.isSkillReady && !bossState.isAttacking)
        {
            if (distFrom_Player >= bossState.defaultAtt_dist)
            {
                bossState.currentState = Boss_State.State.trace;
            }
            else
            {
                bossState.currentState = Boss_State.State.DefaultAtt;
                EachBoss_AttackSetting();
                bossState.isAttacking = true;
            }
        }
    }

    private void UpdateAnimation()
    {
        animCtrl.SetBool("isTrace", bossState.currentState == Boss_State.State.trace);
        animCtrl.SetBool("isAttack", bossState.isAttacking);
        animCtrl.SetInteger("Attack_Type", (int)bossState.currentState);
        
        if (bossState.currentState == Boss_State.State.trace)
        {
            this.transform.rotation = Quaternion.Euler(0, this.transform.position.x > player_pos.x ? 0 : 180, 0);

            Vector2 velo = Vector2.zero;
            this.transform.position = Vector2.SmoothDamp(this.transform.position, player_pos,
                ref velo, move_Speed);
        }
    }

    protected virtual void EachBoss_UpdateSetting() { }
    
    protected virtual void EachBoss_AttackSetting() { }
    
    private void UpdateSkillCooldown()
    {
        if (!bossState.isSkillReady)
        {
            bossState.skill_CountTime += Time.deltaTime;
            if (bossState.skill_CountTime >= bossState.skill_CoolTime)
            {
                bossState.isSkillReady = true;
                bossState.skill_CountTime = 0f;
            }
        }
    }
    // 이벤트 코드 부분

    #region 히트 박스 생성 및 삭제
    public void On_HitArea(int index)
    {
        float thrustValue;
        if (DA_HitArea.Length != 0 && bossState.currentState == Boss_State.State.DefaultAtt)
        {
            TrustValue_Setting(DA_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P1Skill1_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p1_Skill1)
        {
            TrustValue_Setting(P1Skill1_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P1Skill2_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p1_Skill2)
        {
            TrustValue_Setting(P1Skill2_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P2Skill1_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p1_Skill2)
        {
            TrustValue_Setting(P1Skill2_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P2Skill2_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p2_Skill2)
        {
            TrustValue_Setting(P2Skill2_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P2Skill3_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p2_Skill2)
        {
            TrustValue_Setting(P2Skill2_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else return;
    }

    private void TrustValue_Setting(GameObject hitArea)
    {
        float thrustValue;
        thrustValue = hitArea.gameObject.GetComponent<HitColider>().thrustValue;
        thrustValue = thrustValue * (player_pos.x > this.transform.position.x ? 1 : -1);

        hitArea.gameObject.GetComponent<HitColider>().thrustValue = thrustValue;
        hitArea.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    protected virtual void EachBoss_OnHitSetting() {}
    
    public void Off_HitArea(int index)
    {
        if (bossState.currentState == Boss_State.State.DefaultAtt)
        {
            DA_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill1)
        {
            P1Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill2)
        {
            P1Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p2_Skill2)
        {
            P2Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else return;
    }
    #endregion
    
    protected virtual void EachBoss_OffHitSetting() {}

    #region 피격 모션 생성
    public void HitEffect()
    {
        this.gameObject.GetComponent<SpriteRenderer>().material = hit_Mat;
        Invoke("Off_Effect", 0.3f);
    }

    private void Off_Effect()
    {
        this.gameObject.GetComponent<SpriteRenderer>().material = origin_Mat;
    }
    #endregion
    
    protected Boss_State.State Change_IntToState(int selectedBossSkill_int, ref float skillDist)
    {
        if (selectedBossSkill_int == (int)Boss_State.State.p1_Skill1)
        {
            skillDist = bossState.p1_Skill1_dist;
            return Boss_State.State.p1_Skill1;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p1_Skill2)
        {
            skillDist = bossState.p1_Skill2_dist;
            return Boss_State.State.p1_Skill2;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p2_Skill1)
        {
            skillDist = bossState.p2_Skill1_dist;
            return Boss_State.State.p2_Skill1;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p2_Skill2)
        {
            skillDist = bossState.p2_Skill2_dist;
            return Boss_State.State.p2_Skill2;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p2_Skill3)
        {
            skillDist = bossState.p2_Skill3_dist;
            return Boss_State.State.p2_Skill3;
        }
        else return 0;
    }
    
    #region 스킬 종료및 전투 종료 함수
    
    public void EndSkill()
    {
        bossState.currentState = Boss_State.State.trace;
        animCtrl.SetBool("isAttack", false);
        animCtrl.SetBool("isTrace", true);

        bossState.isSkillReady = false;
        bossState.isAttacking = false;
        EachBoss_EndSkill();
        //isHit_Player_fromP2S2 = false;
        
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.State.idle);
    }

    protected virtual void EachBoss_EndSkill() {}
    
    protected virtual void EachBoss_EndAttack() { }

    public void EndAttack()
    {
        bossState.isStopTurn = false;
        
        bossState.isAttacking = false;
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.State.idle);
        EachBoss_EndAttack();
    }

    public void Stop_Turn()
    {
        bossState.isStopTurn = true;
    }
    #endregion
}
