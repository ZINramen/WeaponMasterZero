using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SwordBoss : Boss
{
    #region p1_Skill1_변수 모음
    private Transform skillSpon_Pos;
    [SerializeField] private GameObject GSkill_Pref;
    private GameObject GSkill_dummyObj;
    #endregion
    
    #region p2_Skill1_변수 모음
    private Transform p2_Skill1_MoonPos;
    [SerializeField] private GameObject Curr_BossPos_Pref;
    private GameObject dummy_Obj;

    [SerializeField] private GameObject BladePref;
    private Transform Blade_SponPos;
    private GameObject bladeDummy;
    
    private float minAngle = -70f;
    private float maxAngle = -20f;
    
    private float minSize = 0.3f;
    private float maxSize = 0.6f;
    #endregion

    #region p2_Skill2_변수 모음
    private bool isHit_Player_fromP2S2;
    private float p2s2Att_force;
    [SerializeField] private GameObject bossHit_Area;
    private GameObject dummyArea;
    
    public void Set_HitPlayer_fromP2S2(bool isHit)
    {
        isHit_Player_fromP2S2 = isHit;
    }
    #endregion

    #region p2_Skill3_변수 모음
    [SerializeField] private GameObject Horizontal_LinePref;
    [SerializeField] private GameObject Vertical_LinePref;
    private Transform Right_MaxXY_SponPos;
    private Transform Left_MinXY_SponPos;
    
    private float delete_LineTime;
    private List<GameObject> dummy_LinePref_List = new List<GameObject>();
    
    private float lineAnim_Time;
    private GameObject LinePref_Type;
    
    private bool isReadyCreate_Line;
    private int lineTotalCount;
    private int lineCount;
    private float createLine_Count;
    #endregion
    
    // Start is ca
    // lled before the first frame update
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Sword;
        Init_ValueData();
    }

    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 1f;

        state.skill_CoolTime = 4f;
    
        state.p1_Skill1_dist = 1.5f;
        state.p1_Skill2_dist = 1.8f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
        
        state.p1S1_PossibilityNumber = 0;  // 0  ~ 49  50%
        state.p1S2_PossibilityNumber = 50; // 50 ~ 99  50%
    
        state.p2S1_PossibilityNumber = 0;  // 0 ~ 29    30%
        state.p2S2_PossibilityNumber = 30; // 30 ~ 59   30%
        state.p2S3_PossibilityNumber = 60; // 60 ~ 99   40%
    }

    protected override void Init_ValueData()
    {
        // 이동 관련 변수
        move_Speed = 3f;
        groundApproachDist = 0f;
        
        // 평타 관련 변수
        DA_HitArea = new GameObject[3];
        DA_HitArea[0] = this.transform.GetChild(0).gameObject;
        DA_HitArea[1] = this.transform.GetChild(1).gameObject;
        DA_HitArea[2] = this.transform.GetChild(2).gameObject;

        P1Skill1_HitArea = new GameObject[1];
        P1Skill1_HitArea[0] = this.transform.GetChild(3).gameObject;
        
        P1Skill2_HitArea = new GameObject[2];
        P1Skill2_HitArea[0] = this.transform.GetChild(4).gameObject;
        P1Skill2_HitArea[1] = this.transform.GetChild(5).gameObject;

        P2Skill2_HitArea = new GameObject[1];
        P2Skill2_HitArea[0] = this.transform.GetChild(6).gameObject;
        
        // skill1 충격파 스킬
        skillSpon_Pos = this.transform.GetChild(7).gameObject.transform;
         
        // skill3 칼날 공격 스킬
        p2_Skill1_MoonPos = GameObject.Find("MoonPos").gameObject.transform;
        Blade_SponPos = GameObject.Find("BladeSponPos").gameObject.transform;
         
        minAngle = -100f;
        maxAngle = -25f;
    
        minSize = 0.5f;
        maxSize = 0.7f;
         
        // skill4 칼날 공격 스킬
        p2s2Att_force = 70;
        Right_MaxXY_SponPos = GameObject.Find("Right_MaxXYPos").gameObject.transform;
        Left_MinXY_SponPos = GameObject.Find("Left_MaxXYPos").gameObject.transform;

        delete_LineTime = 0.3f;
        lineAnim_Time = 0.1f;
        lineTotalCount = 20;
    }
    
    // protected override void MoveSetting()
    // {
    //     Vector2 velo = Vector2.zero;
    //     this.transform.position = Vector2.SmoothDamp(this.transform.position, player_pos,
    //         ref velo, move_Speed);
    // }
    
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
        Debug.Log("SetPos");
        
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
            bladeDummy.gameObject.GetComponent<Blade_Ctrl>().OwnerSetting(this.gameObject);
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

        dummyArea = Instantiate(bossHit_Area, new Vector3(0, 0, 0), Quaternion.identity);
        dummyArea.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);

        Vector3 boss_OnlyXpos = new Vector3(this.transform.position.x, -0.83f, this.transform.position.z);
        dummyArea.GetComponent<LineRenderer>().SetPosition(1, boss_OnlyXpos);
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
        EndSkill();
    }
    #endregion

    #region 2p_Skill3 코드
    public void Attack_P2Skill3()
    {
        isReadyCreate_Line = true;
    }

    public void DisAppear()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void End_P2Skill3()
    {
        Debug.Log("EndSkill");
        
        foreach (GameObject dummy_LinePref in dummy_LinePref_List)
            Destroy(dummy_LinePref, delete_LineTime);
        
        Invoke("Appear_Boss", 1f);
        this.transform.position = dummy_Obj.gameObject.transform.position;
        Destroy(dummy_Obj);
        
        this.GetComponent<Rigidbody2D>().simulated = true;

        dummy_LinePref_List.Clear();
        createLine_Count = 0f;
    }

    protected override void EachBoss_UpdateSetting()
    {
        
        if (lineCount < lineTotalCount && isReadyCreate_Line)
        {
            createLine_Count += Time.deltaTime;
            if (createLine_Count >= lineAnim_Time)
            {
                if (lineCount < lineTotalCount / 2) LinePref_Type = Horizontal_LinePref;
                else LinePref_Type = Vertical_LinePref;

                GameObject dummy_LinePref = Instantiate(LinePref_Type, new Vector3(0, 0, 0), quaternion.identity);
                dummy_LinePref.GetComponent<Line_Ctrl>().animationDuration = lineAnim_Time;
                dummy_LinePref.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
                dummy_LinePref.GetComponent<Line_Ctrl>().deleteTime = delete_LineTime;
                dummy_LinePref.GetComponent<Line_Ctrl>().Off_Collider();

                dummy_LinePref.GetComponent<Line_Ctrl>().maxXYPos = Right_MaxXY_SponPos;
                dummy_LinePref.GetComponent<Line_Ctrl>().minXYPos = Left_MinXY_SponPos;
                
                dummy_LinePref.GetComponent<Line_Ctrl>().SwordBoss = this.gameObject;
                
                dummy_LinePref_List.Add(dummy_LinePref);

                lineCount++;
                createLine_Count = 0f;
            }
        }

        else if (lineCount >= lineTotalCount)
        {
            createLine_Count += Time.deltaTime;

            if (createLine_Count >= 1f)
            {
                foreach (GameObject dummy_LinePref in dummy_LinePref_List)
                {
                    dummy_LinePref.GetComponent<Line_Ctrl>().HitMaterial = hit_Mat;
                    dummy_LinePref.GetComponent<Line_Ctrl>().On_Collider();
                }

                foreach (GameObject dummy_LinePref in dummy_LinePref_List)
                    dummy_LinePref.GetComponent<Line_Ctrl>().EndAnimation();

                isReadyCreate_Line = false;
                lineCount = 0;
            }
        }
    }
    #endregion
    
    #region 히트 박스 생성 및 삭제

    protected override void EachBoss_OnHitSetting()
    {
        Debug.Log("EachBoss_OH");
        this.gameObject.GetComponent<Rigidbody2D>().simulated = true;
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(-this.transform.up * p2s2Att_force, ForceMode2D.Impulse);
    }

    protected override void EachBoss_OffHitSetting()
    {
        Debug.Log("EachBoss_OF");
        if (bossState.currentState == Boss_State.State.p2_Skill2)
        {
            Destroy(dummyArea);
        }
    }

    public override void EachBoss_EndSkill()
    {
        Debug.Log("EachBoss_ES");
        isHit_Player_fromP2S2 = false;
        animCtrl.SetBool("isLanding", false);
    }
    #endregion
}
