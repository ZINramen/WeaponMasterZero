using System.Collections;
using System.Collections.Generic;
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

public class LastBoss_Ctrl : Boss
{
    #region 특정 구역에 들어가면, 손 때리는 애니메이션

    [SerializeField] private bool isIn_PlayerArea;

    [SerializeField] private GameObject[] HandAtt_HitArea = new GameObject[4];
    private LastBoss_HandHitAreaPos check_hitAreaPos = LastBoss_HandHitAreaPos.NotHit;
    #endregion
    
    #region 특정 공격만 맞는 패턴 변수
    [SerializeField] private Sprite[] AttackAble_SpriteData = new Sprite[3];
    [SerializeField] private GameObject AttackAble_Sprite;
    
    public enum PlayerAttackType { Sword = 0, Gun, Hammer }
    [SerializeField] private PlayerAttackType attAble_SkillType;

    [SerializeField] private float change_AttTypeTime = 5f;
    [SerializeField] private float change_AttTypeCount;
    #endregion
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Last;
        
        Random_AbleAttType();
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

    private void Random_AbleAttType()
    {
        int randomAttType = Random.Range((int)PlayerAttackType.Sword, (int)PlayerAttackType.Hammer + 1);
        attAble_SkillType = AttAbleSkill_ToEnum(randomAttType);
        
        AttackAble_Sprite.gameObject.GetComponent<SpriteRenderer>().sprite = AttackAble_SpriteData[randomAttType];
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
        change_AttTypeCount += Time.deltaTime;
        if (change_AttTypeCount >= change_AttTypeTime)
        {
            Random_AbleAttType();
            change_AttTypeCount = 0;
        }
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

    public override void EachBoss_EndSkill()
    {
        isIn_PlayerArea = false;
    }

    protected override void EachBoss_EndAttack()
    {
        isIn_PlayerArea = false;
    }
}
