using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class LastBoss_Ctrl : Boss
{
    [SerializeField] private Sprite[] AttackAble_SpriteData = new Sprite[3];
    [SerializeField] private GameObject AttackAble_Sprite;
    
    public enum PlayerAttackType { Sword = 0, Gun, Hammer }
    [SerializeField] private PlayerAttackType attAble_SkillType;

    [SerializeField] private float change_AttTypeTime = 5f;
    [SerializeField] private float change_AttTypeCount;
    
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
        state.defaultAtt_dist = 5000f;

        state.skill_CoolTime = 3f;
    
        state.p1_Skill1_dist = 5000f;
        state.p1_Skill2_dist = 5000f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
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
        
        // 1phaze
        if (bossHP_per >= 0.5f)
        {
            if (selectedNumber >= currState.p1S2_PossibilityNumber)
                iBossSkill = (int)Boss_State.State.p1_Skill2;
            
            else
                iBossSkill = (int)Boss_State.State.p1_Skill1;
            
        }
        
        // 2phaze
        else
        {
            if (selectedNumber >= currState.p2S3_PossibilityNumber)
                iBossSkill = (int)Boss_State.State.p2_Skill3;
            else if(selectedNumber >= currState.p2S2_PossibilityNumber)
                iBossSkill = (int)Boss_State.State.p2_Skill2;
            else
                iBossSkill = (int)Boss_State.State.p2_Skill1;
        }
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
}
