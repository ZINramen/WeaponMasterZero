using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Boss_State
{
    public enum State 
    { 
        trace,
        idle = 0,
        DefaultAtt = 1,
        p1_Skill1 = 2,
        p1_Skill2 = 3,
        p2_Skill1 = 4,
        p2_Skill2 = 5,
        p2_Skill3 = 6 
    }
    public State currentState;

    public float traceDistance = 5000f;
    public float defaultAtt_dist;

    public float skill_CoolTime;

    public float p1_Skill1_dist;
    public float p1_Skill2_dist;
    
    public float p2_Skill1_dist;
    public float p2_Skill2_dist;
    public float p2_Skill3_dist;
    
    public int p1S1_PossibilityNumber;
    public int p1S2_PossibilityNumber;
    
    public int p2S1_PossibilityNumber;
    public int p2S2_PossibilityNumber;
    public int p2S3_PossibilityNumber;

    public bool isAttacking = false;
    public bool isSkillReady = false;
    public float skill_CountTime = 0f;

    public bool isStopTurn = false;
}

public enum BossType { Sword, Gun, Hammer, Last }

public abstract class Boss : MonoBehaviour
{
    protected GameObject player;
    protected Vector2 player_pos;
    
    protected float nextMove;
    protected RaycastHit2D rayHit;
    [SerializeField] protected float groundApproachDist; // 변경 해야함
    protected bool isMove;
    
    public float move_Speed;
    protected Rigidbody2D myRd;
    
    protected Boss_State bossState;
    protected List<Boss_State.State> selectedTurn_State = new List<Boss_State.State>();

    public Boss_State Get_CurrBossState()
    {
        return bossState;
    }
    
    protected float bossHP_per = 1.0f;
    protected float distFrom_Player;
    
    // 처음 시작할 땐, 스킬이 활성화 되어있지 않음
    protected Boss_State.State sBossSkill;
    protected int iBossSkill;
    private bool isSelectSkill = false;
    
    protected float skillDist;
    protected bool isMoveEnd = false;
    #region 충돌 박스 변수 (외부 ref)
    [SerializeField] protected GameObject[] DA_HitArea;
    [SerializeField] protected GameObject[] P1Skill1_HitArea;
    [SerializeField] protected GameObject[] P1Skill2_HitArea;
    [SerializeField] protected GameObject[] P2Skill1_HitArea;
    [SerializeField] protected GameObject[] P2Skill2_HitArea;
    [SerializeField] protected GameObject[] P2Skill3_HitArea;
    #endregion
    
    protected Animator animCtrl;
    private bool isEndSetting = false;

    protected Material origin_Mat;
    [SerializeField] protected Material hit_Mat;
    protected bool isNotHit = false;

    public BossType bossType;
    public bool isDie;
    
    // Start is ca
    // lled before the first frame update
    protected void Start()
    {
        GameObject[] playerTagObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerTagObj in playerTagObjects)
        {
            if (playerTagObj.name == "APO")
                player = playerTagObj;
        }
        
        origin_Mat = this.gameObject.GetComponent<SpriteRenderer>().material;

        myRd = this.gameObject.GetComponent<Movement>().GetBody();
        Move(-move_Speed, -1);
        
        if (player != null)
        {
            player_pos = player.GetComponent<Transform>().position;
        }

        animCtrl = GetComponent<Animator>();

        isMoveEnd = false;
    }

    protected abstract void Init_StateValueData(ref Boss_State state);

    protected virtual void Init_ValueData() {}

    // Update is called once per frame
    protected void Update()
    {
        if (this.gameObject.GetComponent<Entity>().GetHp() > 0 && !isEndSetting)
        {
            UpdateState();
            UpdateAnimation();
            UpdateSkillCooldown();
            EachBoss_UpdateSetting();
        }

        if (isMoveEnd)
        {
            StartCoroutine(StopMove());
            isMoveEnd = false;
        }
            
    }

    protected IEnumerator StopMove()
    {
        while (myRd.velocity.magnitude > 0.5f)
        {
            yield return new WaitForSeconds(0.5f);
            myRd.velocity = Vector2.zero;
        }
    }

    public void MoveXPos_Mingyu(float x)
    {
        x = EachBossMoveSetting(rayHit, x);
        
        int plus = 1;
        if (transform.localEulerAngles.y == 180) plus = -1;
        myRd.AddForce(new Vector2(x * 100 * plus, 0));

        isMoveEnd = true;
    }

    protected virtual float EachBossMoveSetting(RaycastHit2D rayHit, float input_x)
    {
        if (rayHit.collider == null)
            return 0f;

        return input_x;
    }

    private void UpdateState()
    {
        player_pos = player.GetComponent<Transform>().position;
        distFrom_Player = Mathf.Abs(player_pos.x - transform.position.x);
        
        // 상황에 따른 동작 구현 FSM
        if (distFrom_Player >= bossState.traceDistance)
        {
            bossState.currentState = Boss_State.State.idle;
        }
        
        // 스킬 준비가 되어있고, 보스가 공격중이 아니라면.
        else if (bossState.isSkillReady && !bossState.isAttacking)
        {
            if (!isSelectSkill)
            {
                // 보스 체력에 따라, 스킬이 나올것이 달라짐
                bossHP_per = (this.GetComponent<Entity>().GetHp()) / (this.GetComponent<Entity>().maxHP);
                iBossSkill = EachBoss_SelectedSkill(bossState);

                //iBossSkill = (int)Boss_State.State.p1_Skill1;   // # 특정 스킬 지정하기 Test
                
                sBossSkill = Change_IntToState(iBossSkill, ref skillDist);
                Debug.Log("SkillName : " +  sBossSkill);
                isSelectSkill = true;
            }

            if (distFrom_Player >= skillDist)
            {
                bossState.currentState = Boss_State.State.trace;
            }
            else
            {
                myRd.velocity = Vector2.zero;
                
                bossState.currentState = sBossSkill;
                bossState.isAttacking = true;
                bossState.isSkillReady = false;
            }
        }
        
        // 스킬 쿨타임 중일때
        else if (!bossState.isSkillReady && !bossState.isAttacking)
        {
            if (distFrom_Player >= bossState.defaultAtt_dist)
            {
                bossState.currentState = Boss_State.State.trace;
            }
            else
            {
                myRd.velocity = Vector2.zero;
                
                bossState.currentState = Boss_State.State.DefaultAtt;
                EachBoss_AttackSetting();
                bossState.isAttacking = true;
            }
        }
    }

    protected virtual int EachBoss_SelectedSkill(Boss_State currState)
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

    private void UpdateAnimation()
    {
        animCtrl.SetBool("isTrace", bossState.currentState == Boss_State.State.trace);
        animCtrl.SetBool("isAttack", bossState.isAttacking);
        animCtrl.SetInteger("Attack_Type", (int)bossState.currentState);

        if (selectedTurn_State.Count != 0 && selectedTurn_State.Contains(bossState.currentState) &&
            !bossState.isStopTurn)
        {
            this.transform.rotation = Quaternion.Euler(0, this.transform.position.x > player_pos.x ? 0 : 180, 0);
            Debug.Log("Turn N Role");
        }

        else if (bossState.currentState == Boss_State.State.trace)
        {
            Debug.Log("Move");
            this.transform.rotation = Quaternion.Euler(0, this.transform.position.x > player_pos.x ? 0 : 180, 0);
            MoveSetting();
        }
    }
    
    protected virtual void MoveSetting()
    {
        isMove = true;
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

    protected void Move(float inputNextMove, int turnValue)
    {
        myRd.velocity = new Vector2(inputNextMove, myRd.position.y);
        //Debug.Log(myRd.velocity);

        Vector2 frontVec = new Vector2(myRd.position.x + turnValue * groundApproachDist,
            myRd.position.y - 0.5f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 0, 1));      // #Test용
        
        rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));
    }

    protected virtual void EachBoss_UpdateSetting() { }
    
    protected virtual void EachBoss_AttackSetting() { }
    
    private void UpdateSkillCooldown()
    {
        if (!bossState.isSkillReady)
        {
            bossState.skill_CountTime += Time.deltaTime;
            if (bossState.skill_CountTime >= bossState.skill_CoolTime)
            {
                bossState.isSkillReady = true;
                bossState.skill_CountTime = 0f;
            }
        }
    }
    // 이벤트 코드 부분

    #region 히트 박스 생성 및 삭제
    public void On_HitArea(int index)
    {
        if (DA_HitArea.Length != 0 && bossState.currentState == Boss_State.State.DefaultAtt)
        {
            TrustValue_Setting(DA_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P1Skill1_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p1_Skill1)
        {
            TrustValue_Setting(P1Skill1_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P1Skill2_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p1_Skill2)
        {
            TrustValue_Setting(P1Skill2_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P2Skill1_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p2_Skill1)
        {
            TrustValue_Setting(P2Skill1_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P2Skill2_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p2_Skill2)
        {
            TrustValue_Setting(P2Skill2_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else if (P2Skill3_HitArea.Length != 0 && bossState.currentState == Boss_State.State.p2_Skill3)
        {
            TrustValue_Setting(P2Skill3_HitArea[index]);
            EachBoss_OnHitSetting();
        }
        else return;
    }

    public void TrustValue_Setting(GameObject hitArea)
    {
        float thrustValue;
        thrustValue = hitArea.gameObject.GetComponent<HitColider>().thrustValue;
        thrustValue = -Mathf.Abs(thrustValue);
        
        hitArea.gameObject.GetComponent<HitColider>().thrustValue = thrustValue;
        ColliderType(hitArea);
    }

    private void ColliderType(GameObject colliderObj)
    {
        if (colliderObj.GetComponent<BoxCollider2D>() != null)
        {
            colliderObj.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (colliderObj.GetComponent<CircleCollider2D>() != null)
        {
            colliderObj.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        }
        else if (colliderObj.GetComponent<EdgeCollider2D>() != null)
        {
            colliderObj.gameObject.GetComponent<EdgeCollider2D>().enabled = true;
        }
        else
            return;
    }

    protected virtual void EachBoss_OnHitSetting() {}
    
    public void Off_HitArea(int index)
    {
        if (bossState.currentState == Boss_State.State.DefaultAtt)
        {
            DA_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill1)
        {
            P1Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p1_Skill2)
        {
            P1Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p2_Skill1)
        {
            P2Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p2_Skill2)
        {
            P2Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else if (bossState.currentState == Boss_State.State.p2_Skill3)
        {
            P2Skill3_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
            EachBoss_OffHitSetting();
        }
        else return;
    }
    #endregion
    
    protected virtual void EachBoss_OffHitSetting() {}

    #region 피격 모션 생성
    public void HitEffect()
    {
        if (this.gameObject.GetComponent<Entity>().DamageBlock != Entity.DefenseStatus.invincible)
        {
            if (bossType == BossType.Last)
            {
                if (this.gameObject.GetComponent<Entity>().playerFinalBoss)
                    if (player.gameObject.GetComponent<AnimationManager>().ani.GetInteger("Weapon")
                        != this.gameObject.GetComponent<Entity>().desireWeaponFinalBoss)
                        return;

                this.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material = hit_Mat;
            }
            else
                this.gameObject.GetComponent<SpriteRenderer>().material = hit_Mat;
        }

        Invoke("Off_Effect", 0.3f);
    }

    private void Off_Effect()
    {
        if (bossType == BossType.Last)
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material = origin_Mat;
        else
            this.gameObject.GetComponent<SpriteRenderer>().material = origin_Mat;
    }
    #endregion
    
    protected Boss_State.State Change_IntToState(int selectedBossSkill_int, ref float skillDist)
    {
        if (selectedBossSkill_int == (int)Boss_State.State.p1_Skill1)
        {
            skillDist = bossState.p1_Skill1_dist;
            return Boss_State.State.p1_Skill1;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p1_Skill2)
        {
            skillDist = bossState.p1_Skill2_dist;
            return Boss_State.State.p1_Skill2;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p2_Skill1)
        {
            skillDist = bossState.p2_Skill1_dist;
            return Boss_State.State.p2_Skill1;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p2_Skill2)
        {
            skillDist = bossState.p2_Skill2_dist;
            return Boss_State.State.p2_Skill2;
        }
        else if (selectedBossSkill_int == (int)Boss_State.State.p2_Skill3)
        {
            skillDist = bossState.p2_Skill3_dist;
            return Boss_State.State.p2_Skill3;
        }
        else return 0;
    }
    
    #region 스킬 종료및 전투 종료 함수
    
    public void EndSkill()
    {
        isEndSetting = true;
        isSelectSkill = false;

        bossState.isStopTurn = false;
        
        bossState.currentState = Boss_State.State.trace;
        animCtrl.SetBool("isAttack", false);
        animCtrl.SetBool("isTrace", true);

        bossState.isSkillReady = false;
        bossState.isAttacking = false;
        
        EachBoss_EndSkill();
        bossState.skill_CountTime = 0;
        isSelectSkill = false;
        
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.State.idle);
        Invoke("EndSetting", 0.1f);
    }

    public virtual void EachBoss_EndSkill() {}
    
    protected virtual void EachBoss_EndAttack() { }

    public void EndAttack()
    {
        isEndSetting = true;
        isMove = false;
        
        bossState.currentState = Boss_State.State.trace;
        animCtrl.SetBool("isAttack", false);
        animCtrl.SetBool("isTrace", true);
        
        bossState.isStopTurn = false;
        
        bossState.isAttacking = false;
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.State.idle);
        EachBoss_EndAttack();
        
        Invoke("EndSetting", 0.05f);
    }

    private void EndSetting()
    {
        isEndSetting = false;
    }
    
    public void Stop_Turn()
    {
        bossState.isStopTurn = true;
    }
    #endregion
}
