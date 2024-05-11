using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBoss : Boss
{
    [SerializeField] private GameObject General_BulletPref;
    [SerializeField] private GameObject Parrying_BulletPref;
    [SerializeField] private GameObject Piecing_BulletPref;
    
    #region p1_Skill1_변수 모음
    [SerializeField] private GameObject SignPref;
    private Transform SignSpon_Pos;
    private GameObject SignPref_dummyObj;
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
        state.defaultAtt_dist = 0.6f;

        state.skill_CoolTime = 6.0f;
    
        state.p1_Skill1_dist = 5000f;
        state.p1_Skill2_dist = 3f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
    }
    
    #region p1_Skill1_함수
    #endregion
    
    #region p1_Skill2_함수
    #endregion

    #region p2_Skill1_함수
    #endregion

    #region p2_Skill3_함수
    #endregion
    
    #region 히트 박스 생성 및 삭제
    protected override void EachBoss_OnHitSetting()
    {
    }

    protected override void EachBoss_OffHitSetting()
    {
    }

    protected override void EachBoss_EndSkill()
    {
    }
    #endregion
}
