using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public enum LastBoss_FistAttType
{
    UpFist = 0,
    RightFist = 1,
    DownFist = 2,
    LeftFist = 3,
}

public class LastBoss_Ctrl : Boss
{
    /* 1. 손날치기                    (기본 평타)
       2. 중력 스킬                   (skill 1)
       3. 입에서 탄막 발사             (skill 2)
       4. 흡수 공격                   (skill 3)
       5. 땅 내려 찍기
       6. 영역 안에 있으면, 주먹 발사
       
       5. 체력이 50% 이하일 때, 특정 배경으로 바뀌더니, 특정 무기의 공격만 들어감
       6. 체력이 60% 30%일 때, 자물쇠 패턴이 나오고 딜이 넣어지지 않음 */

    #region 기본 평타 변수
    private bool isSelect_DAttType = false;
    private enum  DAtt_Type
    {
        LeftAtt = 0,
        RightAtt = 1
    }
    
    private bool isLeftAtt;
    private int dAttType_int;
    #endregion

    #region 중력 변수
    [SerializeField] private float playerJump_Power;
    #endregion
    
    #region 탄막 발사 변수 Many Bullet용 // skill2
    [SerializeField] private GameObject ShootingBullet_Pref;
    [SerializeField] private Transform ShootingBulletPos;
    [SerializeField] private int bulletCount;                   // 한번 등장하는 총알 갯수
    [SerializeField] private int shooting_AttTotalCount;        // 공격 횟수
    
    private float bullet_Degree = 0f;
    private bool isActive_Shooting = false;
    private int shooting_AttCount = 0;         // 공격 횟수 카운트
    
    private enum  FireAtt_Type
    {
        ManyAtt = 0,
        TwoAtt = 1
    }
    
    private bool isShooting_ManyAtt;

    private float delayTime = 0.5f;
    private float delayCount = 0f;

    private GameObject dummyBullet;
    #endregion
    
    #region 흡수 스킬 변수
    [SerializeField] private GameObject AbsorbHand;
    private GameObject dummy_AbsorbHand;
    private bool isActive_Absorb;
    
    private GameObject LeftWall;
    private GameObject RightWall;
    private GameObject UpWall;
    private GameObject[] WallObjs;
    
    private float left_SponPos;
    private float right_SponPos;
    private float SponYPos;

    [SerializeField] private float absorbAtt_DelayTime;
    [SerializeField] private int AbsorbAtt_TotalCount;
    [SerializeField] private float absorbHP_Amount;
    [SerializeField] private float downSpeed;
    
    private float absorbAtt_DelayCount;
    private int absorbAtt_Count;
    private int delete_AbsorbCount;
    #endregion
    
    #region 특정 구역에 들어가면, 손 때리는 공격 변수
    private LastBoss_FistAttType fistAttType;

    [SerializeField] private GameObject FistObj;
    [SerializeField] private Transform leftUP_FistSponPos;
    [SerializeField] private Transform rightDown_FistSponPos;

    [SerializeField] private int fistTotalCount;
    [SerializeField] private float fistDelayTime;
    [SerializeField] private float fistPower;
    public int delete_FistCount;

    private int fistCount;
    private float fistDelayCount;
    private bool isActive_Fist;
    
    private GameObject dummyFist;
    #endregion
    
    #region 특정 공격만 맞는 패턴 변수
    private Entity LastBoss_Entity;
    private float changeAttack_AbleHP;
    
    [SerializeField] private Sprite[] AttackAble_BackGroundData = new Sprite[3];
    [SerializeField] private GameObject AttackAble_BackGround;
    
    public enum PlayerAttackType { Sword = 1, Gun, Hammer, NotSetting }
    [SerializeField] private PlayerAttackType attAble_AttType;
    private int random_AttAbleType;

    [SerializeField] private float change_AttTypeTime = 5f;
    private float change_AttTypeCount;
    #endregion

    #region 자물쇠 패턴
    private bool isAble_firstLockHP = false;
    private bool isAble_secondLockHP = false;

    private float firstLockHP;
    private float secondLockHP;
    
    [SerializeField] private GameObject BubbleKey_Obj;
    [SerializeField] private GameObject LockObj;
    [SerializeField] private float m_bubblePower;
    private BubbleData currentBubble;
    
    public BubbleData Get_BubbleData() { return currentBubble; }

    private GameObject dummy_BubbleKey_Obj;
    private GameObject dummy_LockObj;
    #endregion
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Last;

        LastBoss_Entity = this.gameObject.GetComponent<Entity>();
        changeAttack_AbleHP = LastBoss_Entity.maxHP / 2;
        //changeAttack_AbleHP = LastBoss_Entity.maxHP;        // Test
        
        /* 체력의 60%와 30% 일때, 자물쇠 패턴을 사용 */
        //firstLockHP = LastBoss_Entity.maxHP * 0.6f;
        
        firstLockHP = LastBoss_Entity.maxHP * 1f;           // Test
        secondLockHP = LastBoss_Entity.maxHP * 0.3f;

        #region 벽의 위치를 구해, 스폰될 구역을 만드는 로직
        WallObjs = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in WallObjs)
        {
            if (wall.gameObject.name.Contains("Left")) LeftWall = wall;
            else if (wall.gameObject.name.Contains("Right")) RightWall = wall;
            else if (wall.gameObject.name.Contains("Up")) UpWall = wall;
        }

        left_SponPos =  (Mathf.Abs(LeftWall.transform.position.x) - (LeftWall.transform.localScale.x / 2) - 1f) * -1 + 3f;
        right_SponPos =  RightWall.transform.position.x - (RightWall.transform.localScale.x / 2) - 7f;
        SponYPos = UpWall.transform.position.y;
        #endregion
        
        attAble_AttType = PlayerAttackType.NotSetting;
        random_AttAbleType = (int)PlayerAttackType.NotSetting;

        bullet_Degree = 360f / bulletCount;
    }
    
    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 5000f;      // 손날 치기

        state.skill_CoolTime = 3f;
    
        state.p1_Skill1_dist = 5000f;       // 중력
        state.p1_Skill2_dist = 5000f;       // 탄막 발싸
        state.p2_Skill1_dist = 5000f;       // 흡수 공격
        state.p2_Skill2_dist = 5000f;       // 땅을 내려찍어, 가시가 튀어나오게 함
        state.p2_Skill3_dist = 5000f;       // 특정 구역에 들어가면, 주먹으로 때림
        
        state.p1S1_PossibilityNumber = 0;   // 0  ~ 19  20%
        state.p1S2_PossibilityNumber = 20;  // 20 ~ 39  20%
        state.p2S1_PossibilityNumber = 40;  // 40 ~ 59  20%
        state.p2S2_PossibilityNumber = 60;  // 60 ~ 79  20%
        state.p2S3_PossibilityNumber = 80;  // 80 ~ 100 20%
    }
    
    protected override void EachBoss_AttackSetting()
    {
        if (!isSelect_DAttType)
        {
            isSelect_DAttType = true;

            dAttType_int = Random.Range((int)DAtt_Type.LeftAtt, (int)DAtt_Type.RightAtt + 1);
            if (dAttType_int == (int)DAtt_Type.LeftAtt) isLeftAtt = true;
            else isLeftAtt = false;

            animCtrl.SetBool("is_DAtt_L", isLeftAtt);
        }
    }
    
    protected override int EachBoss_SelectedSkill(Boss_State currState)
    {
        int selectedNumber;
        selectedNumber = Random.Range(0, 100);      // 0 ~ 99

        if (selectedNumber >= currState.p2S3_PossibilityNumber)
            iBossSkill = (int)Boss_State.State.p2_Skill3;
        
        else if(selectedNumber >= currState.p2S2_PossibilityNumber)
            iBossSkill = (int)Boss_State.State.p2_Skill2;

        else if(selectedNumber >= currState.p2S1_PossibilityNumber)
            iBossSkill = (int)Boss_State.State.p2_Skill1;
        
        else if (selectedNumber >= currState.p1S2_PossibilityNumber)
        {
            Select_ShootingType();
            iBossSkill = (int)Boss_State.State.p1_Skill2;
        }
        
        else
            iBossSkill = (int)Boss_State.State.p1_Skill1;
        
        return iBossSkill;
    }

    protected override void EachBoss_UpdateSetting()
    {
        // 체력이 50% 이하라면, 공격타입을 결정할 수 있게 진행
        if (LastBoss_Entity.GetHp() <= changeAttack_AbleHP)
        {
            if (attAble_AttType == PlayerAttackType.NotSetting)
                Setting_AbleAttType();
            
            change_AttTypeCount += Time.deltaTime;
            if (change_AttTypeCount >= change_AttTypeTime)
            {
                Random_AbleAttType();
                change_AttTypeCount = 0;
            }
        }

        if (LastBoss_Entity.GetHp() <= firstLockHP && !isAble_firstLockHP)
        {
            animCtrl.SetTrigger("LockAtt");
            isAble_firstLockHP = true;
        }
        else if (LastBoss_Entity.GetHp() <= secondLockHP && !isAble_secondLockHP)
        {
            animCtrl.SetTrigger("LockAtt");
            isAble_secondLockHP = true;
        }

        if (isActive_Shooting)
        {
            if(shooting_AttCount < shooting_AttTotalCount)
            {
                delayCount += Time.deltaTime;

                if (delayCount >= delayTime)
                {
                    ShootingBullet();
                    delayCount = 0;
                }
            }
            else
            {
                isActive_Shooting = false;

                animCtrl.SetBool("isShootingAtt", false);
                Invoke("EndSkill", 0.5f);
            }
        }

        if (isActive_Absorb && absorbAtt_Count < AbsorbAtt_TotalCount)
        {
            absorbAtt_DelayCount += Time.deltaTime;

            if (absorbAtt_DelayCount >= absorbAtt_DelayTime)
            {
                Spon_AbsorbHand();
                
                absorbAtt_DelayCount = 0;
                absorbAtt_Count++;
            }
        }
        
        else if (absorbAtt_Count >= AbsorbAtt_TotalCount 
                 && delete_AbsorbCount >= AbsorbAtt_TotalCount)
        {
            isActive_Absorb = false;
            animCtrl.SetBool("isEnd_Absorb", true);
        }

        if (isActive_Fist && fistCount < fistTotalCount)
        {
            fistDelayCount += Time.deltaTime;

            if (fistDelayCount >= fistDelayTime)
            {
                int random_FistAttType = Random.Range
                    ((int)LastBoss_FistAttType.UpFist, (int)LastBoss_FistAttType.LeftFist + 1);

                fistAttType = ChangeTo_intFistType(random_FistAttType);
                AttackFist(fistAttType);

                fistCount++;
                fistDelayCount = 0;
            }
        }
        else if (fistCount >= fistTotalCount 
                 && delete_FistCount >= fistTotalCount)
        {
            isActive_Fist = false;
            animCtrl.SetBool("isEnd_Fist", true);
        }
    }
    #region 중력 공격 skill 1 함수
    public void AttackGravity_Skill1()
    {
        player.gameObject.GetComponent<Movement>().Jump(playerJump_Power);
    }

    public void DownAttack()
    {
        player.gameObject.GetComponent<Movement>().body.AddForce
            (-Vector2.up * playerJump_Power / 2 , ForceMode2D.Impulse);
    }
    #endregion
    
    #region 탄막 발사 skill 2 함수
    public void AttackShooting_Skill2()
    {
        isActive_Shooting = true;
        
        animCtrl.SetBool("isShootingAtt", true);
        ShootingBullet();
    }
    
    public void ShootingBullet()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float bulletAngle;
            if (shooting_AttCount % 2 == 0) bulletAngle = bullet_Degree * i;
            else bulletAngle = bullet_Degree / 2 + bullet_Degree * i;

            Quaternion randomRotation = Quaternion.Euler(0, 0, bulletAngle);

            // 프리팹 생성
            dummyBullet = Instantiate(ShootingBullet_Pref, ShootingBulletPos.position, randomRotation);
            dummyBullet.gameObject.GetComponent<LastBoss_BulletHitColl>().owner = LastBoss_Entity;
            dummyBullet.transform.GetChild(0).gameObject.GetComponent<HitColider>().owner = LastBoss_Entity;
            dummyBullet.gameObject.GetComponent<BulletCtrl>().install_ZValue = bulletAngle;
        }

        shooting_AttCount++;
    }

    public void Select_ShootingType()
    {
        int shootingType = Random.Range((int)FireAtt_Type.ManyAtt, (int)FireAtt_Type.TwoAtt + 1);
        if (dAttType_int == (int)FireAtt_Type.ManyAtt) isShooting_ManyAtt = true;
        else isShooting_ManyAtt = false;

        isShooting_ManyAtt = false; // Test

        animCtrl.SetBool("is_ManyFire", isShooting_ManyAtt);
    }
    #endregion
    
    #region 흡수 공격 skill 3 함수
    public void AttackAbsorb_Skill3()
    {
        isActive_Absorb = true;
    }

    private void Spon_AbsorbHand()
    {
        float absorbHand_SponXPos = Random.Range(left_SponPos, right_SponPos);
        Vector2 sponPos = new Vector2(absorbHand_SponXPos, SponYPos);

        dummy_AbsorbHand = Instantiate(AbsorbHand, sponPos, Quaternion.identity);

        Absorb_HitCol[] absorbHand_Arr = dummy_AbsorbHand.transform.GetComponentsInChildren<Absorb_HitCol>();

        foreach (Absorb_HitCol absorbHand in absorbHand_Arr)
        {
            absorbHand.owner = LastBoss_Entity;
            absorbHand.SetAddHP(absorbHP_Amount);
        }
        
        dummy_AbsorbHand.gameObject.GetComponent<Rigidbody2D>().AddForce
            (Vector2.down * downSpeed, ForceMode2D.Impulse);
    }

    public void Delete_AbsorbHand()
    {
        delete_AbsorbCount++;
    }
    #endregion

    #region 랜덤으로, 주먹으로 공격
    public void AttackFist_Skill6()
    {
        isActive_Fist = true;
    }

    private LastBoss_FistAttType ChangeTo_intFistType(int attackType_int)
    {
        LastBoss_FistAttType current_FistAttType = LastBoss_FistAttType.UpFist;
        
        switch (attackType_int)
        {
            case (int)LastBoss_FistAttType.UpFist:
                current_FistAttType = LastBoss_FistAttType.UpFist;
                break;
            
            case (int)LastBoss_FistAttType.RightFist: 
                current_FistAttType = LastBoss_FistAttType.RightFist;
                break;
            
            case (int)LastBoss_FistAttType.DownFist:
                current_FistAttType = LastBoss_FistAttType.DownFist;
                break;
            
            case (int)LastBoss_FistAttType.LeftFist: 
                current_FistAttType = LastBoss_FistAttType.LeftFist;
                break;
            
            default:
                Debug.Log("ㅈ버그");
                break;
        }

        return current_FistAttType;
    }

    private void AttackFist(LastBoss_FistAttType input_fistAttType)
    {
        Debug.Log("생성 및 공격");
        Vector2 FistSponPos = Vector2.zero;
        
        switch (input_fistAttType)
        {
            case LastBoss_FistAttType.UpFist:
                FistSponPos.x = Random.Range
                    (leftUP_FistSponPos.position.x + 2f, rightDown_FistSponPos.position.x - 2f);
                FistSponPos.y = rightDown_FistSponPos.position.y;
                
                SponFist(FistSponPos, -90f, Vector2.up);
                break;
            
            case LastBoss_FistAttType.RightFist:
                FistSponPos.x = leftUP_FistSponPos.position.x;
                FistSponPos.y = Random.Range
                    (rightDown_FistSponPos.position.y + 3f, leftUP_FistSponPos.position.y - 3f);
                
                SponFist(FistSponPos, 0, Vector2.right);
                break;
            
            case LastBoss_FistAttType.DownFist:
                FistSponPos.x = Random.Range
                    (leftUP_FistSponPos.position.x + 2f, rightDown_FistSponPos.position.x - 2f);
                FistSponPos.y = leftUP_FistSponPos.position.y;
                
                SponFist(FistSponPos, 90f, Vector2.down);
                break;
            
            case LastBoss_FistAttType.LeftFist:
                FistSponPos.x = rightDown_FistSponPos.position.x;
                FistSponPos.y = Random.Range
                    (rightDown_FistSponPos.position.y + 3f, leftUP_FistSponPos.position.y - 3f);
                
                SponFist(FistSponPos, 0, Vector2.left);
                break;
            
            default: 
                Debug.Log("개버그 2");
                return;
        }
    }

    private void SponFist(Vector2 FistSponPos, float rotation, Vector2 PowerPos)
    {
        dummyFist = GameObject.Instantiate(FistObj, FistSponPos, Quaternion.identity);
        dummyFist.transform.rotation = Quaternion.Euler(0,0, rotation);
        if (PowerPos == Vector2.right)
            dummyFist.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = true;
        
        dummyFist.gameObject.GetComponent<FistHitCol>().owner = LastBoss_Entity;
        
        dummyFist.gameObject.GetComponent<Rigidbody2D>().AddForce
            (PowerPos * fistPower, ForceMode2D.Impulse);
    }

    public void DeleteFist()
    {
        delete_FistCount++;
    }
    #endregion
    
    #region 배경 변경 skill 6 함수
    private void Setting_AbleAttType()
    {
        Random_AbleAttType();

        LastBoss_Entity.activeDesireWeapon = true;
        LastBoss_Entity.playerFinalBoss = player.gameObject.GetComponent<Entity>();
    }
    
    private void Random_AbleAttType()
    {
        random_AttAbleType = Random.Range((int)PlayerAttackType.Sword, (int)PlayerAttackType.Hammer + 1);
        attAble_AttType = AttAbleSkill_ToEnum(random_AttAbleType);
        
        AttackAble_BackGround.gameObject.GetComponent<SpriteRenderer>().sprite = AttackAble_BackGroundData[random_AttAbleType - 1];
        LastBoss_Entity.desireWeaponFinalBoss = random_AttAbleType;
    }

    private PlayerAttackType AttAbleSkill_ToEnum(int input_AttAbleType)
    {
        if (input_AttAbleType == (int)PlayerAttackType.Sword)
            return PlayerAttackType.Sword;
        
        else if (input_AttAbleType == (int)PlayerAttackType.Gun)
            return PlayerAttackType.Gun;
        
        else
            return PlayerAttackType.Hammer;
    }
    #endregion
    
    #region 자물쇠 skill 7 함수
    public void Active_LockSkill()
    {
        Vector2 BubbleKey_SponPos = Vector2.zero;
        
        currentBubble = new BubbleData();
        currentBubble.Set_SponAblePos(leftUP_FistSponPos, rightDown_FistSponPos);
        currentBubble.SetBubbleData(ref BubbleKey_SponPos);
        currentBubble.bubblePower = m_bubblePower;
        
        dummy_BubbleKey_Obj = Instantiate(BubbleKey_Obj, BubbleKey_SponPos, Quaternion.identity);
        dummy_BubbleKey_Obj.gameObject.GetComponent<BubbleKey_Ctrl>().SetBubbleData(ref currentBubble);
        dummy_BubbleKey_Obj.gameObject.GetComponent<BubbleKey_Ctrl>().Player = player;
        
        dummy_LockObj = Instantiate(LockObj, player.gameObject.transform.position, Quaternion.identity);
        dummy_LockObj.transform.parent = player.gameObject.transform;

        this.gameObject.GetComponent<Entity>().DamageBlock = Entity.DefenseStatus.invincible;
    }
    #endregion

    public override void EachBoss_EndSkill()
    {
        isSelect_DAttType = false;
        isActive_Absorb = false;
        
        shooting_AttCount = 0;
        absorbAtt_Count = 0;
        delete_AbsorbCount = 0;
        
        fistCount = 0;
        delete_FistCount = 0;
        
        animCtrl.SetBool("isEnd_Absorb", false);
        animCtrl.SetBool("isEnd_Fist", false);
    }

    protected override void EachBoss_EndAttack()
    {
        isSelect_DAttType = false;
    }
}
