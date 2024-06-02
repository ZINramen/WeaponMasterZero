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
    
    [SerializeField] private GameObject Shock_wave;
    private Transform ShockWave_SponPos;
    private float sW_AttackPower;
    
    private GameObject dummyShock_waveL;
    private GameObject dummyShock_waveR;
    #endregion
    
    #region P1_Skill1
    private GameObject[] FallGrounds;
    private int fallGround_index = 0;
    private int fallGround_TotalCount = 3;

    private float shakePower;
    #endregion
    
    #region P1_Skill2
    private bool is_PermanentMove;
    private float p1_Skill2_MoveSpeed;
    private float originSpeed;
    #endregion

    #region P2_Skill1
    [SerializeField] private GameObject SnowPref;
    private GameObject RushTrail;
    
    private float rushSpeed;
    private float rush_GroundInterval = 2f;
    private float origin_GroundInterval;
    private bool isRush_P2S1 = false;
    
    private Transform SnowSponPos;
    private float snowForce;
    private float p2S1_DelayTime;
    
    private GameObject dummy_SnowObj;
    #endregion
    
    #region P2_Skill2
    [SerializeField] private GameObject StonPref; 
    private Transform StonSponPos;
    private float stonForce;
    private GameObject dummy_StonObj;
    #endregion
    
    #region P2_Skill3
    [SerializeField] private GameObject IceRain_Pref;
    private GameObject dummy_IceRainObj;
    
    private GameObject LeftWall;
    private GameObject RightWall;
    private GameObject[] WallObjs;

    private Transform P2S3_SponYPos;
    private float left_SponPos;
    private float right_SponPos;
    private float iceRainGravity;
    
    private int p2S3_IceRainTotalCount;
    private int p2S3_IceRainCount;
    
    private float p2S3_AttTime;
    private float p2S3_DelayTime;
    private float p2S3_DelayCount;

    private bool isActiveSkill_p2S3 = false;
    #endregion
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Hammer;
        selectedTurn_State.Add(Boss_State.State.DefaultAtt);
        selectedTurn_State.Add(Boss_State.State.p1_Skill1);
        selectedTurn_State.Add(Boss_State.State.p2_Skill1);

        Init_ValueData();

        RushTrail.gameObject.SetActive(false);
        
        WallObjs = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in WallObjs)
        {
            if (wall.gameObject.name.Contains("Left")) LeftWall = wall;
            else if (wall.gameObject.name.Contains("Right")) RightWall = wall;
        }

        left_SponPos =  (Mathf.Abs(LeftWall.transform.position.x) - (LeftWall.transform.localScale.x / 2) - 1f) * -1f;
        right_SponPos =  RightWall.transform.position.x - (RightWall.transform.localScale.x / 2) - 1f;
        
        originSpeed = move_Speed;
        origin_GroundInterval = groundApproachDist;
    }
    
    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 1.4f;

        state.skill_CoolTime = 1f;
    
        state.p1_Skill1_dist = 6f;
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
        groundApproachDist = 3f;
        
        // 평타 관련 변수
        DA_HitArea = new GameObject[3];
        DA_HitArea[0] = this.transform.GetChild(0).gameObject;
        DA_HitArea[1] = this.transform.GetChild(1).gameObject;
        DA_HitArea[2] = this.transform.GetChild(2).gameObject;
        
        ShockWave_SponPos = this.transform.GetChild(3).gameObject.transform;

        P1Skill1_HitArea = new GameObject[4];
        P1Skill1_HitArea[0] = this.transform.GetChild(5).gameObject;
        P1Skill1_HitArea[1] = this.transform.GetChild(6).gameObject;
        P1Skill1_HitArea[2] = this.transform.GetChild(7).gameObject;
        P1Skill1_HitArea[3] = this.transform.GetChild(8).gameObject;
        
        P1Skill2_HitArea = new GameObject[1];
        P1Skill2_HitArea[0] = this.transform.GetChild(10).gameObject;
        
        P2Skill1_HitArea = new GameObject[1];
        P2Skill1_HitArea[0] = this.transform.GetChild(12).gameObject;
        RushTrail = this.transform.GetChild(13).gameObject;
        
        P2Skill2_HitArea = new GameObject[1];
        P2Skill2_HitArea[0] = this.transform.GetChild(15).gameObject;
        
        P2Skill3_HitArea = new GameObject[1];
        P2Skill3_HitArea[0] = this.transform.GetChild(0).gameObject;
        
        SnowSponPos = this.transform.GetChild(17).gameObject.transform;
        StonSponPos = this.transform.GetChild(18).gameObject.transform;

        // 충격파 (강 공격)
        sW_AttackPower = 10f;
        
        // 땅 떨구기 Skill1
        FallGrounds = new GameObject[3];
        GameObject FallGrounds_Parent = GameObject.Find("Grid").gameObject;
        FallGrounds[0] = FallGrounds_Parent.transform.GetChild(0).gameObject;
        FallGrounds[1] = FallGrounds_Parent.transform.GetChild(1).gameObject;
        FallGrounds[2] = FallGrounds_Parent.transform.GetChild(2).gameObject;

        shakePower = 1f;

        // 뱅글 뱅글 패턴 Skill2
        p1_Skill2_MoveSpeed = 7f;
        
        // 눈덩이 굴리기 패턴
        rushSpeed = 12f;
        snowForce = 100000f;
        p2S1_DelayTime = 1.5f;
        rush_GroundInterval = 2f;
        
        // 돌 던지기 패턴
        stonForce = 40f;
        
        // 고드름 떨구기
        P2S3_SponYPos = GameObject.Find("P2_Skill3_SponYPos").gameObject.transform;
        p2S3_IceRainTotalCount = 20;
        p2S3_AttTime = 2f;
        isActiveSkill_p2S3 = false;
    }

    #region 평타 관련 함수
    public void DAttack_SponShockWave()
    {
        dummyShock_waveL = Instantiate(Shock_wave, ShockWave_SponPos.position, quaternion.identity);
        dummyShock_waveR = Instantiate(Shock_wave, ShockWave_SponPos.position, quaternion.identity);

        dummyShock_waveL.gameObject.GetComponent<ShowWave_HitCollider>().owner = this.gameObject.GetComponent<Entity>();
        dummyShock_waveR.gameObject.GetComponent<ShowWave_HitCollider>().owner = this.gameObject.GetComponent<Entity>();
        
        dummyShock_waveL.gameObject.GetComponent<Rigidbody2D>().AddForce(sW_AttackPower * Vector2.left, ForceMode2D.Impulse);
        dummyShock_waveR.gameObject.GetComponent<Rigidbody2D>().AddForce(sW_AttackPower * Vector2.right, ForceMode2D.Impulse);
    }
    
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
        x = groundApproachDist * -30;
        
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
            FallGrounds[fallGround_index].gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            //FallGrounds[fallGround_index].transform.parent.GetComponent<FallGroundSize>().FallGround();
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
    public void Rush_P2S1()
    {
        isRush_P2S1 = true;
        RushTrail.gameObject.SetActive(true);
        groundApproachDist = rush_GroundInterval;

        this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.invincible;
    }
    
    public void Stop_Trail()
    {
        RushTrail.gameObject.SetActive(false);
        this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.Nope;
    }
    
    public void AttackP2_S1()
    {
        groundApproachDist = origin_GroundInterval;
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        
        dummy_SnowObj = Instantiate(SnowPref, SnowSponPos.position, quaternion.identity);
        dummy_SnowObj.transform.GetChild(0).gameObject.GetComponent<SnowBall_HitColl>().owner = this.gameObject.GetComponent<Entity>();
        
        dummy_SnowObj.gameObject.GetComponent<Rigidbody2D>().AddForce(
            (this.transform.localEulerAngles.y == 180 ? Vector2.right : Vector2.left) * snowForce, ForceMode2D.Impulse);
        
        Invoke("EndSkill", p2S1_DelayTime);
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
    
    #region P2_Skill3 함수
    public void AttackSettingP2_S3()
    {
        p2S3_DelayTime = p2S3_AttTime / (float)p2S3_IceRainTotalCount;
        isActiveSkill_p2S3 = true;
        IceRain();
        
        this.gameObject.GetComponent<Animator>().SetBool("isActive_IceRain", true);
        this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y == 180 ? 0 : 180, 0);
    }

    public void IceRain()
    {
        float iceRain_SponXPos = Random.Range(left_SponPos, right_SponPos);
        Vector2 sponPos = new Vector2(iceRain_SponXPos, P2S3_SponYPos.position.y);

        dummy_IceRainObj = Instantiate(IceRain_Pref, sponPos, Quaternion.identity);
        dummy_IceRainObj.gameObject.GetComponent<StonHitColl>().owner = this.gameObject.GetComponent<Entity>();

        float randomGravityScale = Random.Range(1f, 2f);
        dummy_IceRainObj.gameObject.GetComponent<Rigidbody2D>().gravityScale = randomGravityScale;
    }

    public void CheckEndSkillP2_S3()
    {
        if (p2S3_IceRainCount >= p2S3_IceRainTotalCount)
        {
            isActiveSkill_p2S3 = false;
            p2S3_DelayCount = 0f;
            p2S3_IceRainCount = 0;
            
            this.gameObject.GetComponent<Animator>().SetBool("isActive_IceRain", false);
            Invoke("EndSkill", 0.5f);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y == 180 ? 0 : 180, 0);
        }
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

        if (isRush_P2S1)
        {
            rushSpeed = this.transform.rotation.eulerAngles.y == 180 ? 
                    Mathf.Abs(rushSpeed) : -Mathf.Abs(rushSpeed);
        
            if (rayHit.collider != null)
            {
                Move(rushSpeed, rushSpeed > 0 ? 1 : -1);
            }
            else
            {
                Move(0, nextMove > 0 ? 1 : -1);
                this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y == 180 ? 0 : 180, 0);
                this.gameObject.GetComponent<Animator>().SetBool("isEndRush_P2S1", true);
                isRush_P2S1 = false;
            }
        }

        if (isActiveSkill_p2S3 
            && p2S3_IceRainCount < p2S3_IceRainTotalCount)
        {
            p2S3_DelayCount += Time.deltaTime;

            if (p2S3_DelayCount >= p2S3_DelayTime)
            {
                IceRain();
                
                p2S3_DelayCount = 0;
                p2S3_IceRainCount++;
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
        
        this.gameObject.GetComponent<Animator>().SetBool("isEndRush_P2S1", false);
        Stop_Trail();
    }
    #endregion
}
