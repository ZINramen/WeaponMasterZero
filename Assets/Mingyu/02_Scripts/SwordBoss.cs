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
    
    [SerializeField] private float delete_LineTime = 0.3f;
    private List<GameObject> dummy_LinePref_List = new List<GameObject>();
    
    [SerializeField] private float lineAnim_Time;
    private GameObject LinePref_Type;
    
    private bool isReadyCreate_Line;
    private int lineCount = 0;
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
    }

    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 1f;

        state.skill_CoolTime = 4.0f;
    
        state.p1_Skill1_dist = 1.5f;
        state.p1_Skill2_dist = 1.8f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
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
        
        this.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("End_P2Skill3", 2f);
    }

    private void End_P2Skill3()
    {
        foreach (GameObject dummy_LinePref in dummy_LinePref_List)
        {
            dummy_LinePref.GetComponent<Line_Ctrl>().HitMaterial = hit_Mat;
            dummy_LinePref.GetComponent<Line_Ctrl>().On_Collider();
        }

        foreach (GameObject dummy_LinePref in dummy_LinePref_List)
            Destroy(dummy_LinePref, delete_LineTime);
        
        Invoke("Appear_Boss", 1f);
        this.transform.position = dummy_Obj.gameObject.transform.position;
        Destroy(dummy_Obj);
        
        this.GetComponent<Rigidbody2D>().simulated = true;

        dummy_LinePref_List.Clear();
        isReadyCreate_Line = false;
        createLine_Count = 0f;
        lineCount = 0;
    }

    protected override void EachBoss_UpdateSetting()
    {
        if (lineCount < 6 && isReadyCreate_Line)
        {
            createLine_Count += Time.deltaTime;
            if (createLine_Count >= lineAnim_Time)
            {
                if (lineCount < 3) LinePref_Type = Horizontal_LinePref;
                else               LinePref_Type = Vertical_LinePref;
                
                GameObject dummy_LinePref = Instantiate(LinePref_Type, new Vector3(0, 0, 0), quaternion.identity);
                dummy_LinePref.GetComponent<Line_Ctrl>().animationDuration = lineAnim_Time;
                dummy_LinePref.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
                dummy_LinePref.GetComponent<Line_Ctrl>().Off_Collider();
                dummy_LinePref_List.Add(dummy_LinePref);

                lineCount++;
                createLine_Count = 0f;
            }
        }
    }
    #endregion
    
    #region 히트 박스 생성 및 삭제

    protected override void EachBoss_OnHitSetting()
    {
        this.gameObject.GetComponent<Rigidbody2D>().simulated = true;
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(-this.transform.up * p2s2Att_force, ForceMode2D.Impulse);
    }

    protected override void EachBoss_OffHitSetting()
    {
        if (bossState.currentState == Boss_State.State.p2_Skill2)
        {
            Destroy(dummyArea);
        }
    }

    public override void EachBoss_EndSkill()
    {
        isHit_Player_fromP2S2 = false;
        animCtrl.SetBool("isLanding", false);
    }
    #endregion
}
