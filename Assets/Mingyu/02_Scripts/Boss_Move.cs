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
    public float defaultAtt_dist = 1f;

    public float skill_CoolTime = 4.0f;
    
    public float p1_Skill1_dist = 1.5f;
    public float p1_Skill2_dist = 1.8f;
    
    public float p2_Skill1_dist = 5000f;
    public float p2_Skill2_dist = 5000f;
    public float p2_Skill3_dist = 5000f;

    public bool isAttacking = false;
    public bool isSkillReady = false;
    public float skill_CountTime = 0f;

    public bool isStopTurn = false;
}

public class Boss_Move : MonoBehaviour
{
    private GameObject player;
    private Vector2 player_pos;

    public float move_Speed;
    private Boss_State bossState;

    public Boss_State Get_CurrBossState()
    {
        return bossState;
    }
    
    private float bossHP_per = 1.0f;
    private float distFrom_Player;
    
    // 처음 시작할 땐, 스킬이 활성화 되어있지 않음
    private Boss_State.State sBossSkill;
    private int iBossSkill;
    
    private float skillDist;

    #region p1_Skill1_변수 모음
    [SerializeField] private Transform skillSpon_Pos;
    [SerializeField] private GameObject GSkill_Pref;
    private GameObject GSkill_dummyObj;
    #endregion
    
    #region p2_Skill1_변수 모음
    [SerializeField] private Transform p2_Skill1_MoonPos;
    [SerializeField] private GameObject Curr_BossPos_Pref;
    private GameObject dummy_Obj;

    [SerializeField] private GameObject BladePref;
    [SerializeField] private Transform Blade_SponPos;
    private GameObject bladeDummy;
    
    [SerializeField] public float minAngle = -70f;
    [SerializeField] public float maxAngle = -20f;
    
    [SerializeField] public float minSize = 0.3f;
    [SerializeField] public float maxSize = 0.6f;
    #endregion

    #region p2_Skill2_변수 모음
    private bool isHit_Player_fromP2S2;
    [SerializeField] private float p2s2Att_force;
    
    public void Set_HitPlayer_fromP2S2(bool isHit)
    {
        isHit_Player_fromP2S2 = isHit;
    }
    #endregion
    
    #region 충돌 박스 변수 (외부 ref)
    [SerializeField] private GameObject[] DA_HitArea;
    [SerializeField] private GameObject[] P1Skill1_HitArea;
    [SerializeField] private GameObject[] P1Skill2_HitArea;
    [SerializeField] private GameObject[] P2Skill2_HitArea;
    private int index = 0;
    #endregion
    
    private Animator animCtrl;

    private Material origin_Mat;
    [SerializeField] private Material hit_Mat;
    
    // Start is ca
    // lled before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").gameObject;
        origin_Mat = this.gameObject.GetComponent<SpriteRenderer>().material;
        
        if (player != null)
        {
            player_pos = player.GetComponent<Transform>().position;
        }

        animCtrl = GetComponent<Animator>();
        bossState = new Boss_State();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
        UpdateAnimation();
        UpdateSkillCooldown();
    }

    private void UpdateState()
    {
        player_pos = player.GetComponent<Transform>().position;
        distFrom_Player = Vector2.Distance(player_pos, transform.position);
        
        // 보스 체력에 따라, 스킬이 나올것이 달라짐
        bossHP_per = ( this.GetComponent<Entity>().GetHp() ) / ( this.GetComponent<Entity>().maxHP );
        if (bossHP_per >= 0.5f)
            iBossSkill = Random.Range((int)Boss_State.State.p1_Skill1, (int)Boss_State.State.p1_Skill2 + 1);
        else
            iBossSkill = Random.Range((int)(int)Boss_State.State.p2_Skill1, (int)Boss_State.State.p2_Skill3);
        iBossSkill = 2;
        
        sBossSkill = Change_IntToState(iBossSkill, ref skillDist);
        
        // 상황에 따른 동작 구현 FSM
        if (distFrom_Player >= bossState.traceDistance)
        {
            bossState.currentState = Boss_State.State.idle;
        }
        
        // 스킬 준비가 되어있고, 보스가 공격중이 아니라면.
        else if (bossState.isSkillReady && !bossState.isAttacking)
        {
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

    #region 1p_Skill1 코드

    public void Make_GSkill()
    {
        float current_TrunValue = (this.transform.rotation.y == -1 ? 0 : -1);
        Debug.Log("플레이어 방향 : " + this.transform.rotation.y);
        // 왼쪽 = 0 <-> 오른쪽 1

        GSkill_Pref.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
        
        float thrustValue = GSkill_Pref.gameObject.GetComponent<HitColider>().thrustValue;
        thrustValue = thrustValue * this.transform.rotation.y == -1 ? 1 : -1;

        GSkill_Pref.gameObject.GetComponent<HitColider>().thrustValue = thrustValue;
        
        GSkill_dummyObj = Instantiate(GSkill_Pref, skillSpon_Pos.position, Quaternion.identity);
        GSkill_dummyObj.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(0, current_TrunValue * 180, 0);
        Debug.Log("충격파 방향 : " + current_TrunValue * 180);
        
        Invoke("Delete_GSkillDummyObj", 1f);
    }

    private void Delete_GSkillDummyObj()
    {
        Destroy(GSkill_dummyObj);
    }
    #endregion
    
    #region 2p_Skill1 코드
    public void SetPos_P2Skill1()
    {
        dummy_Obj = Instantiate(Curr_BossPos_Pref, this.transform.position, Quaternion.identity);
        this.transform.position = p2_Skill1_MoonPos.position;
        this.GetComponent<Rigidbody2D>().simulated = false;
    }

    public void Attack_P2Skill1()
    {
        for (int i = 0; i < 6; i++)
        {
            float RandomData = (Random.insideUnitSphere).x;

            // -20 ~ -5
            // -35 ~ -20
            // -50 ~ -35
            // -65 ~ -50
            // -80 ~ -65
            // -95 ~ -80
            float randomAngle = Random.Range(minAngle + (i * 15), maxAngle + (i * 15));
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);
            
            float randomSize = Random.Range(minSize, maxSize);
            
            // 프리팹 생성
            bladeDummy = Instantiate(BladePref, Blade_SponPos.position, randomRotation);
            bladeDummy.transform.localScale = new Vector3(randomSize, randomSize, 1);
        }

        this.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("End_P2Skill1", 2f);
    }

    private void End_P2Skill1()
    {
        Invoke("Appear_Boss", 1f);
        this.transform.position = dummy_Obj.gameObject.transform.position;
        Destroy(dummy_Obj);
        
        this.GetComponent<Rigidbody2D>().simulated = true;
    }
    
    private void Appear_Boss()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
        animCtrl.SetBool("isLanding", true);
    }
    #endregion

    #region 2p_Skill2 코드
    public void SetPos_P2S2()
    {
        this.GetComponent<Rigidbody2D>().simulated = false;
        this.transform.position = new Vector2(player_pos.x, p2_Skill1_MoonPos.transform.position.y);
    }
    
    public void HitCheck_2PSkill2()
    {
        // 플레이어가 맞았다면, 모션 끝
        if (isHit_Player_fromP2S2)
        {
            Debug.Log("맞았음");
            
            animCtrl.SetBool("isFixed", false);
            EndSkill();
        }

        // 플레이어가 맞지 않았다면, 스스로 기절
        else
        {
            Debug.Log("안 맞았음");
            
            animCtrl.SetBool("isFixed", true);
        }
    }

    public void End_FixedSwordMotion()
    {
        animCtrl.SetBool("isFixed", false);
        EndAttack();
        EndSkill();
    }
    #endregion

    #region 히트 박스 생성 및 삭제
    public void On_HitArea(int index)
    {
        float thrustValue;
        if (bossState.currentState == Boss_State.State.DefaultAtt)
        {
            thrustValue = DA_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            thrustValue = thrustValue * (player_pos.x > this.transform.position.x ? 1 : -1);
            
            DA_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue;
            DA_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill1)
        {
            thrustValue = P1Skill1_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            thrustValue = thrustValue * (player_pos.x > this.transform.position.x ? 1 : -1);

            P1Skill1_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue;
            P1Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill2)
        {
            thrustValue = P1Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            thrustValue = thrustValue * (player_pos.x > this.transform.position.x ? 1 : -1);

            P1Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue;
            P1Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (bossState.currentState == Boss_State.State.p2_Skill2)
        {
            thrustValue = P2Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            thrustValue = thrustValue * (player_pos.x > this.transform.position.x ? 1 : -1);

            P2Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue;
            P2Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;

            this.gameObject.GetComponent<Rigidbody2D>().simulated = true;
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(-this.transform.up * p2s2Att_force, ForceMode2D.Impulse);
        }
        else return;
    }
    
    public void Off_HitArea(int index)
    {
        if (bossState.currentState == Boss_State.State.DefaultAtt)
        {
            DA_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill1)
        {
            P1Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill2)
        {
            P1Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (bossState.currentState == Boss_State.State.p2_Skill2)
        {
            P2Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else return;
    }
    #endregion

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
    
    private Boss_State.State Change_IntToState(int selectedBossSkill_int, ref float skillDist)
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
    
    private void EndSkill()
    {
        bossState.currentState = Boss_State.State.trace;
        animCtrl.SetBool("isAttack", false);
        animCtrl.SetBool("isTrace", true);

        bossState.isSkillReady = false;
        bossState.isAttacking = false;
        isHit_Player_fromP2S2 = false;
        
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.State.idle);
    }

    public void EndAttack()
    {
        bossState.isStopTurn = false;
        
        bossState.isAttacking = false;
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.State.idle);
    }

    public void Stop_Turn()
    {
        bossState.isStopTurn = true;
    }
    
    private void EndSetting()
    {
        EndSkill();
        EndAttack();
    }
    #endregion
}
