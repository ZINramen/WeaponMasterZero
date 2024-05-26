using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum LastBoss_HandHitAreaPos
{
    UpArea = 0,
    RightArea = 1,
    DownArea = 2,
    LeftArea = 3,
    NotHit = 4
}

public class LastBoss_State : Boss_State
{
    
}

public class LastBoss_Ctrl : Boss
{
    /* 1. 주먹으로 내려찍음             (기본 평타)
       2. 땅에 불을 뿜음               (skill 1)
       3. 입에서 탄막 발사             (skill 2)
       4. 특정 구역에 가면 손이 공격함   (skill 3)
       
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
    [SerializeField] private int shooting_AttCount = 0;         // 공격 횟수 카운트
    
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
    
    #region  // skill2

    

    #endregion
    
    #region 특정 구역에 들어가면, 손 때리는 공격 변수

    [SerializeField] private bool isIn_PlayerArea;

    [SerializeField] private GameObject[] HandAtt_HitArea = new GameObject[4];
    private LastBoss_HandHitAreaPos check_hitAreaPos = LastBoss_HandHitAreaPos.NotHit;
    
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
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Last;

        LastBoss_Entity = this.gameObject.GetComponent<Entity>();
        //changeAttack_AbleHP = LastBoss_Entity.maxHP / 2;
        changeAttack_AbleHP = LastBoss_Entity.maxHP;
        
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
        Select_ShootingType();                      // Test

        HitCheck_HandArea();

        if (!isIn_PlayerArea && selectedNumber >= currState.p2S2_PossibilityNumber)
            selectedNumber = Random.Range(0, currState.p2S3_PossibilityNumber);     // 0 ~ 80 사이를 다시 돌림   20%
        
        else if (selectedNumber >= currState.p2S3_PossibilityNumber)
        {
            iBossSkill = (int)Boss_State.State.p2_Skill2;
            animCtrl.SetInteger("Random_HandAttPos", (int)check_hitAreaPos);
        }
        
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
    }

    #region 기본 평타 함수
    public void Attack_Default()
    {
        
    }
    #endregion
    
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
    
    #region 손 공격 skill 3 함수
    public void AttackHand_Skill3()
    {
        
    }
    
    private void HitCheck_HandArea()
    {
        foreach (GameObject hitArea in HandAtt_HitArea)
        {
            if (hitArea.gameObject.GetComponent<LastBoss_HitAreaCheck>().isHit_Player == true)
            {
                Debug.Log("에리어 안");
                
                isIn_PlayerArea = true;
                check_hitAreaPos = hitArea.gameObject.GetComponent<LastBoss_HitAreaCheck>().hitAreaPos;
                return;
            }
            isIn_PlayerArea = false;
            check_hitAreaPos = LastBoss_HandHitAreaPos.NotHit;
        }
    }
    #endregion
    
    #region 배경 변경 skill 4 함수
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
    
    #region 자물쇠 skill 5 함수

    

    #endregion

    public override void EachBoss_EndSkill()
    {
        isIn_PlayerArea = false;
        isSelect_DAttType = false;
        shooting_AttCount = 0;
    }

    protected override void EachBoss_EndAttack()
    {
        isSelect_DAttType = false;
        isIn_PlayerArea = false;
    }
}
