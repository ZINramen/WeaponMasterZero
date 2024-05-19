using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class HammerBoss : Boss
{
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

    [SerializeField] private float shakePower;
    #endregion
    
    #region P1_Skill2
    private bool is_PermanentMove;
    [SerializeField] private float p1_Skill2_MoveSpeed;
    private float originSpeed;
    #endregion

    #region P2_Skill1
    [SerializeField] private GameObject SnowPref;
    [SerializeField] private Transform SnowSponPos;
    [SerializeField] private float snowForce;
    private GameObject dummy_SnowObj;
    #endregion
    
    #region P2_Skill2
    [SerializeField] private GameObject StonPref;
    [SerializeField] private Transform StonSponPos;
    [SerializeField] private float stonForce;
    private GameObject dummy_StonObj;
    #endregion
    
    #region P2_Skill2
    private GameObject LeftWall;
    private GameObject RightWall;
    private GameObject[] WallObjs;

    [SerializeField] private float left_SponPos;
    [SerializeField] private float right_SponPos;
    
    [SerializeField] private float iceRainForce;
    private List<GameObject> dummy_iceRain_List;
    #endregion
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Hammer;
        selectedTurn_State.Add(Boss_State.State.DefaultAtt);
        selectedTurn_State.Add(Boss_State.State.p1_Skill1);

        dummy_iceRain_List = new List<GameObject>();
        
        WallObjs = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in WallObjs)
        {
            if (wall.gameObject.name.Contains("Left")) LeftWall = wall;
            else if (wall.gameObject.name.Contains("Right")) RightWall = wall;
        }

        left_SponPos =  (Mathf.Abs(LeftWall.transform.position.x) - LeftWall.transform.localScale.x - 1f) * -1f;
        right_SponPos =  RightWall.transform.position.x - RightWall.transform.localScale.x - 1f;
        
        originSpeed = move_Speed;
    }
    
    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 1.4f;

        state.skill_CoolTime = 1f;
    
        state.p1_Skill1_dist = 2f;
        state.p1_Skill2_dist = 1.8f;
    
        state.p2_Skill1_dist = 5f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
    }

    #region 평타 관련 함수

    public void StartTurn()
    {
        bossState.isStopTurn = false;
    }
    #endregion

    #region P1_Skill1 함수
    public void Jump_Mingyu(float JumpPower)
    {
        if (myRd.bodyType == RigidbodyType2D.Static)
            return;
        
        myRd.velocity = new Vector2(myRd.velocity.x, JumpPower * 2);
    }
    
    protected override float EachBossMoveSetting(RaycastHit2D rayHit, float x)
    {
        Move(0, this.transform.position.x > player_pos.x ? -1 : 1);
        Debug.Log("Check");
        
        // 내려찍는 패턴에서 ray cast값을 통해 collider값을 확인하여, 넘어가면 반대로 뛰어록 설정
        // 해당 값이 가장 안정적임
        x = (groundApproachDist - 2f) * -1;
        
        if (rayHit.collider == null 
            && bossState.currentState == Boss_State.State.p1_Skill1)
        {
            Debug.Log("Stop");
            Stop_Turn();
            this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y == 180 ? 0 : 180, 0);
        }
        return x;
    }
    
    public void Attack_HP1S1()
    {
        Camera.main.gameObject.GetComponent<DynamicCamera>().ShakeScreen(shakePower);
        FallGround();
    }

    private void FallGround()
    {
        if (fallGround_index >= fallGround_TotalCount)
            return;
        
        else if (FallGrounds[fallGround_index].gameObject.activeSelf == true)
        {
            FallGrounds[fallGround_index].gameObject.GetComponent<Rigidbody2D>().simulated = true;
            FallGrounds[fallGround_index].transform.parent.GetComponent<FallGroundSize>().FallGround();
            Invoke("Delete_FallGround", 0.3f);
        }
    }

    private void Delete_FallGround()
    {
        FallGrounds[fallGround_index].gameObject.GetComponent<Rigidbody2D>().simulated = false;
        FallGrounds[fallGround_index].gameObject.SetActive(false);
        fallGround_index++;
    }
    #endregion

    #region P1_Skill2 함수
    public void AttackP1_S2()
    {
        move_Speed = p1_Skill2_MoveSpeed;
        is_PermanentMove = true;
    }
    
    public void EndAttackP1_S2()
    {
        is_PermanentMove = false;
        StartCoroutine(StopMove());
    }
    #endregion
    
    #region P2_Skill1 함수
    public void AttackP2_S1()
    {
        dummy_SnowObj = Instantiate(SnowPref, SnowSponPos.position, quaternion.identity);
        dummy_SnowObj.transform.GetChild(0).gameObject.GetComponent<SnowBall_HitColl>().owner = this.gameObject.GetComponent<Entity>();
        
        dummy_SnowObj.gameObject.GetComponent<Rigidbody2D>().AddForce(
            (this.transform.localEulerAngles.y == 180 ? 
                Vector2.right : Vector2.left) * snowForce, ForceMode2D.Impulse);
    }
    #endregion
    
    #region P2_Skill2 함수
    public void AttackP2_S2()
    {
        dummy_StonObj = Instantiate(StonPref, StonSponPos.position, quaternion.identity);
        dummy_StonObj.gameObject.GetComponent<StonHitColl>().owner = this.gameObject.GetComponent<Entity>();
        
        dummy_StonObj.gameObject.GetComponent<Rigidbody2D>().AddForce(
            (this.transform.localEulerAngles.y == 180 ? 
                Vector2.right : Vector2.left) * stonForce, ForceMode2D.Impulse);
    }
    #endregion
    
    #region P2_Skill2 함수
    public void AttackP2_S3()
    {
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
            //isFullAtt = true; // Test

            animCtrl.SetBool("isFullAtt", isFullAtt);
        }
    }

    protected override void EachBoss_UpdateSetting()
    {
        if (is_PermanentMove)
        {
            this.transform.rotation = Quaternion.Euler(0, this.transform.position.x > player_pos.x ? 0 : 180, 0);
            
            nextMove = this.transform.position.x > player_pos.x ? -move_Speed : move_Speed;
        
            if (rayHit.collider != null)
            {
                Move(nextMove, nextMove > 0 ? 1 : -1);
            }
            else
            {
                Move(0, nextMove > 0 ? 1 : -1);
            }
        }
    }
    
    #region 엔드 세팅
    protected override void EachBoss_EndAttack()
    {
        isSelect_DAttType = false;
    }

    public override void EachBoss_EndSkill()
    {
        move_Speed = originSpeed;
        isSelect_DAttType = false;
    }
    #endregion
}
