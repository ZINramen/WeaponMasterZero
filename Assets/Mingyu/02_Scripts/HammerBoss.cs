using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBoss : Boss
{
    #region 움직이는 것 관련 변수
    private Rigidbody2D myRd;
    private float nextMove = 1;
    
    [SerializeField] private float groundApproachDist;
    [SerializeField] private float moveSpeed;
    private RaycastHit2D rayHit;
    #endregion
    
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
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Hammer;
        myRd = this.gameObject.GetComponent<Rigidbody2D>();
        Move(-moveSpeed, -1);
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

    protected override void MoveSetting()
    {
        nextMove = this.transform.position.x > player_pos.x ? -moveSpeed : moveSpeed;
        Debug.Log("움직임");
        
        if (rayHit.collider != null)
        {
            Move(nextMove, nextMove > 0 ? 1 : -1);
        }
        else
        {
            Move(0, nextMove > 0 ? 1 : -1);
        }
    }

    private void Move(float inputNextMove, int turnValue)
    {
        myRd.velocity = new Vector2(inputNextMove, myRd.position.y);

        Vector2 frontVec = new Vector2(myRd.position.x + turnValue * groundApproachDist, myRd.position.y - 0.5f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 0, 1));      // #Test용
        
        rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));
    }
    
    protected override void EachBoss_AttackSetting()
    {
        if (!isSelect_DAttType)
        {
            isSelect_DAttType = true;

            dAttType_int = Random.Range((int)DAtt_Type.DefaultAtt, (int)DAtt_Type.FullAtt + 1);
            if (dAttType_int == (int)DAtt_Type.FullAtt) isFullAtt = true;
            else isFullAtt = false;

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
