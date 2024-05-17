using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBoss : Boss
{
    [SerializeField] private float groundApproachDist;
    
    #region 평타 관련 변수 모음
    private bool isSelect_DAttType = false;
    private enum  DAtt_Type
    {
        DefaultAtt = 0,
        FullAtt = 1
    }

    private bool isFullAtt;
    private int dAttType_int;
    #endregion

    #region P1_Skill1
    [SerializeField] private GameObject[] FallGrounds = new GameObject[3];
    private int fallGround_index = 0;
    private int fallGround_TotalCount = 3;
    #endregion
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Hammer;
    }
    
    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 1.4f;

        state.skill_CoolTime = 6f;
    
        state.p1_Skill1_dist = 1.5f;
        state.p1_Skill2_dist = 1.8f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
    }

    #region P1_Skill1
    public void Attack_P1S1(float x)
    {

        
        FallGround(fallGround_index);
        fallGround_index++;
    }

    private void FallGround(int index)
    {
        if (index >= fallGround_TotalCount)
            return;
        
        else if (FallGrounds[index].gameObject.activeSelf == true)
            FallGrounds[index].gameObject.GetComponent<Animator>().SetBool("isFall", true);
    }
    #endregion
    
    protected override void EachBoss_AttackSetting()
    {
        if (!isSelect_DAttType)
        {
            isSelect_DAttType = true;

            dAttType_int = Random.Range((int)DAtt_Type.DefaultAtt, (int)DAtt_Type.FullAtt + 1);
            if (dAttType_int == (int)DAtt_Type.FullAtt) isFullAtt = true;
            else isFullAtt = false;
            isFullAtt = true; // Test

            animCtrl.SetBool("isFullAtt", isFullAtt);
        }
    }
    
    #region 엔드 세팅
    protected override void EachBoss_EndAttack()
    {
        isSelect_DAttType = false;
    }

    public override void EachBoss_EndSkill()
    {
        isSelect_DAttType = false;
    }
    #endregion
}
