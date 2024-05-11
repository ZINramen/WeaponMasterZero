using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunBoss : Boss
{
    [SerializeField] private GameObject General_BulletPref;
    [SerializeField] private GameObject Parrying_BulletPref;
    [SerializeField] private GameObject Piecing_BulletPref;
    
    #region 평타 관련 변수 모음

    private bool isSelect_DAttType = false;
    private enum  DAtt_Type
    {
        SpinAtt = 0,
        DownAtt = 1
    }

    private bool isSpinAtt;
    private int dAttType_int;
    #endregion
    
    #region p1_Skill1_변수 모음
    [SerializeField] private GameObject SignPref;
    Transform SignSpon_Pos;
    private List<GameObject> SignPref_dummyObjList = new List<GameObject>();
    
    private const int signAllCount = 6;
    private int signCount = 0;
    
    [SerializeField] private float signSpon_DelayTime = 0.5f;
    private float signSpon_WaitTime = 0f;
    private bool isReady_CreateSign = false;
    #endregion
    
    #region p1_Skill2_변수 모음
    [SerializeField] private float boomAtt_delayTime;
    [SerializeField] private Transform Dynamite_SponPos;
    [SerializeField] private GameObject DynamitePref;
    private GameObject DynamitePref_dummyObj;
    #endregion

    #region p2_Skill1_변수 모음
    [SerializeField] private GameObject InstallPref;
    [SerializeField] private Transform[] Install_SponPos;
    private GameObject InstallPref_dummyObj;
    #endregion

    #region p2_Skill3_변수 모음
    [SerializeField] private Transform[] SixBullet_SponPos;
    #endregion
    
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
        state.defaultAtt_dist = 1f;

        state.skill_CoolTime = 6.0f;
    
        state.p1_Skill1_dist = 5000f;
        state.p1_Skill2_dist = 3f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
    }
    
    #region p1_Skill1_함수
    public void Setting_p1Skill1()
    {
        isReady_CreateSign = true;

        Invoke("Attack_p1Skill1", signSpon_DelayTime * 6 + 0.5f);
    }

    public void Attack_p1Skill1()
    {
        animCtrl.SetBool("isSignAtt", true);
        isReady_CreateSign = false;
        
        foreach (GameObject dummy_signPref in SignPref_dummyObjList)
        {
            dummy_signPref.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
            dummy_signPref.GetComponent<CircleCollider2D>().enabled = true;
        }

        End_P1Skill1();
    }

    private void Create_AttSign()
    {
        Transform playerPos = player.transform;
        GameObject dummyObj = Instantiate(SignPref, playerPos.position, quaternion.identity);
        dummyObj.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
        SignPref_dummyObjList.Add(dummyObj);
    }

    private void End_P1Skill1()
    {
        animCtrl.SetBool("isSignAtt", false);
        
        foreach (GameObject dummy_signPref in SignPref_dummyObjList)
        {
            Destroy(dummy_signPref);
        }
    }
    
    #endregion
    
    #region p1_Skill2_함수
    #endregion

    #region p2_Skill1_함수
    #endregion

    #region p2_Skill3_함수
    #endregion

    protected override void EachBoss_UpdateSetting()
    {
        if (isReady_CreateSign && signCount < signAllCount)
        {
            signSpon_WaitTime += Time.deltaTime;

            if (signSpon_WaitTime >= signSpon_DelayTime)
            {
                Create_AttSign();
                
                signSpon_WaitTime = 0;
                signCount++;
            }
        }
    }

    protected override void EachBoss_AttackSetting()
    {
        if (!isSelect_DAttType)
        {
            isSelect_DAttType = true;

            dAttType_int = Random.Range((int)DAtt_Type.SpinAtt, (int)DAtt_Type.DownAtt + 1);
            if (dAttType_int == 1) isSpinAtt = false;
            else isSpinAtt = true;

            animCtrl.SetBool("isSpinAtt", isSpinAtt);
        }
    }
    
    #region 히트 박스 생성 및 삭제
    protected override void EachBoss_OnHitSetting()
    {
    }

    protected override void EachBoss_OffHitSetting()
    {
    }
    
    protected override void EachBoss_EndAttack()
    {
        isSelect_DAttType = false;
    }

    protected override void EachBoss_EndSkill()
    {
        isSelect_DAttType = false;
        
        SignPref_dummyObjList.Clear();
        signCount = 0;
    }
    #endregion
}
