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
    
    #region 탄막 발사 변수 // skill2
    [SerializeField] private GameObject ShootingBullet_Pref;
    [SerializeField] private int bulletCount;                   // 한번 등장하는 총알 갯수
    [SerializeField] private int shooting_AttTotalCount;        // 공격 횟수
    
    private float bullet_Degree = 0f;
    private bool isActive_Shooting = false;
    [SerializeField] private int shooting_AttCount = 0;                          // 공격 횟수 카운트

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
        state.defaultAtt_dist = 5000f;      // 내려찍어 충격파 만들기

        state.skill_CoolTime = 3f;
    
        state.p1_Skill1_dist = 5000f;       // 불 뿜기
        state.p1_Skill2_dist = 5000f;       // 탄막 발싸
        state.p2_Skill1_dist = 5000f;       // 하늘에서 막대 떨어뜨리기 (플레이어가 맞으면 보스 피 회복)
        state.p2_Skill2_dist = 5000f;       // 특정 위치에 도달하면, 손 뻗어서 공격
        
        state.p1S1_PossibilityNumber = 0;   // 0  ~ 24  25%
        state.p1S2_PossibilityNumber = 25;  // 25 ~ 49  25%
        state.p2S1_PossibilityNumber = 50;  // 50 ~ 74  25%
        state.p2S2_PossibilityNumber = 75;  // 75 ~ 100 25%
    }
    
    protected override int EachBoss_SelectedSkill(Boss_State currState)
    {
        int selectedNumber;
        selectedNumber = Random.Range(0, 100);      // 0 ~ 99

        HitCheck_HandArea();

        if (!isIn_PlayerArea && selectedNumber >= currState.p2S2_PossibilityNumber)
            selectedNumber = Random.Range(0, currState.p2S2_PossibilityNumber);     // 0 ~ 75 사이를 다시 돌림   25%
            
        else if(selectedNumber >= currState.p2S2_PossibilityNumber)
            iBossSkill = (int)Boss_State.State.p2_Skill2;

        else if(selectedNumber >= currState.p2S1_PossibilityNumber)
            iBossSkill = (int)Boss_State.State.p2_Skill1;
        
        else if (selectedNumber >= currState.p1S2_PossibilityNumber)
            iBossSkill = (int)Boss_State.State.p1_Skill2;
        
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
    
    #region 불뿜기 skill 1 함수
    public void AttackFire_Skill1()
    {
        
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
            dummyBullet = Instantiate(ShootingBullet_Pref, this.gameObject.transform.position, randomRotation);
            dummyBullet.gameObject.GetComponent<LastBoss_BulletHitColl>().owner = LastBoss_Entity;
            dummyBullet.gameObject.GetComponent<BulletCtrl>().install_ZValue = bulletAngle;
        }

        shooting_AttCount++;
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
        shooting_AttCount = 0;
    }

    protected override void EachBoss_EndAttack()
    {
        isIn_PlayerArea = false;
    }
}
