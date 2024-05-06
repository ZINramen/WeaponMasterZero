using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Boss_Move : MonoBehaviour
{
    private GameObject player;
    private Vector2 player_pos;

    public float move_Speed;

    public enum Boss_State
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

    private int trace_Dist = 5000;
    [SerializeField] private float defaultAtt_dist = 1f;

    [SerializeField] private float p1_Skill1_dist = 1.5f;
    [SerializeField] private float p1_Skill2_dist = 1.8f;

    // 광역기
    private float p2_Skill1_dist = 5000f;
    private float p2_Skill2_dist = 5000f;
    private float p2_Skill3_dist = 5000f;

    private Boss_State current_State = Boss_State.idle;

    public Boss_State Get_CurrBossState()
    {
        return current_State;
    }
    
    private float bossHP_per = 1.0f;
    [SerializeField] private const float skill_CoolTime = 4.0f;
    private float distFrom_Player;
    
    // 처음 시작할 땐, 스킬이 활성화 되어있지 않음
    private float skill_CountTime = 0f;
    private bool isCoolTime = true;
    private bool isOnSkill = false;
    private bool isAttack = false;

    private Boss_State sBossSkill;
    private int iBossSkill;
    private float skillDist;
    private bool flapX;

    [SerializeField] private float waitTime = 1f;

    #region p1_Skill1_변수 모음
    [SerializeField] private Transform skillSpon_Pos;
    [SerializeField] private GameObject GSkill_Pref;
    #endregion
    
    #region p2_Skill1_변수 모음
    [SerializeField] private Transform p2_Skill1_MoonPos;
    [SerializeField] private GameObject Curr_BossPos_Pref;
    private GameObject dummy_Obj;

    [SerializeField] private GameObject BladePref;
    [SerializeField] private Transform Blade_SponPos;
    private GameObject bladeDummy;
    
    [SerializeField] public float minAngle = -70f;
    [SerializeField] public float maxAngle = -20f;
    
    [SerializeField] public float minSize = 0.3f;
    [SerializeField] public float maxSize = 0.6f;
    #endregion

    #region p2_Skill2_변수 모음
    private bool isHit_Player_fromP2S2;
    [SerializeField] private float p2s2Att_force;
    
    public void Set_HitPlayer_fromP2S2(bool isHit)
    {
        isHit_Player_fromP2S2 = isHit;
    }
    #endregion
    
    #region 충돌 박스 변수 (외부 ref)
    [SerializeField] private GameObject[] DA_HitArea;
    [SerializeField] private GameObject[] P1Skill1_HitArea;
    [SerializeField] private GameObject[] P1Skill2_HitArea;
    [SerializeField] private GameObject[] P2Skill2_HitArea;
    private int index = 0;
    #endregion
    
    private Animator animCtrl;

    private Material origin_Mat;
    [SerializeField] private Material hit_Mat;
    
    // Start is ca
    // lled before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").gameObject;
        origin_Mat = this.gameObject.GetComponent<SpriteRenderer>().material;
        
        if (player != null)
        {
            player_pos = player.GetComponent<Transform>().position;
        }

        animCtrl = GetComponent<Animator>();
        current_State = Boss_State.idle;

        StartCoroutine(Check_MonsterState());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player_pos = player.GetComponent<Transform>().position;
        flapX= (this.transform.position.x - player_pos.x) > 0 ? true : false;

        if (!flapX) this.transform.rotation = Quaternion.Euler(0, 180, 0);
        else this.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (current_State == Boss_State.trace && isAttack == false)
        {
            Vector2 velo = Vector2.zero;
            this.transform.position = Vector2.SmoothDamp( this.transform.position, player_pos, 
                ref velo, move_Speed);
        }

        if ( isCoolTime && skill_CountTime >= skill_CoolTime )
        {
            bossHP_per = ( this.GetComponent<Entity>().GetHp() ) / ( this.GetComponent<Entity>().maxHP );
            if (bossHP_per >= 0.5f)
                iBossSkill = Random.Range((int)Boss_State.p1_Skill1, (int)Boss_State.p1_Skill2 + 1);
            else
                iBossSkill = Random.Range((int)Boss_State.p2_Skill1, (int)Boss_State.p2_Skill3);

            iBossSkill = 3;

            sBossSkill = Change_IntToState(iBossSkill, ref skillDist);
            isCoolTime = false;
        }
        else if(!isOnSkill)
        {
            skill_CountTime += Time.fixedDeltaTime;
        }
    }

    IEnumerator Check_MonsterState()
    {
        while (true)
        {
            distFrom_Player = Vector2.Distance(player_pos, this.transform.position);

            // 추적 사거리 이상이면, Idle
            if (distFrom_Player >= trace_Dist)
            {
                Debug.Log("1");
                current_State = Boss_State.idle;
                animCtrl.SetBool("isTrace", false);
                animCtrl.SetBool("isAttack", false);
            }

            // 스킬 쿨타임이 돌았는지 확인
            else if (!isCoolTime && isAttack == false)
            {
                
                if (distFrom_Player >= skillDist)
                {
                    current_State = Boss_State.trace;
                    
                    animCtrl.SetBool("isTrace", true);
                    animCtrl.SetBool("isAttack", false);
                    animCtrl.SetInteger("Attack_Type", (int)Boss_State.idle);
                }
                else
                {
                    isAttack = true;
                    
                    Debug.Log("3");
                    current_State = sBossSkill;

                    animCtrl.SetBool("isAttack", true);
                    animCtrl.SetBool("isTrace", false);
                    animCtrl.SetInteger("Attack_Type", iBossSkill);
                    
                    isOnSkill = true;
                }
            }
            
            // 스킬 쿨타임이 돌지 않았다면, 평타
            else if(!isOnSkill && isAttack == false)
            {
                //Debug.Log("Dist : " + distFrom_Player);
                
                if (distFrom_Player >= defaultAtt_dist)
                {
                    Debug.Log("4");
                    current_State = Boss_State.trace;
                    
                    animCtrl.SetBool("isTrace", true);
                    animCtrl.SetBool("isAttack", false);
                }
                else 
                {
                    isAttack = true;
                    
                    current_State = Boss_State.DefaultAtt;
                    animCtrl.SetBool("isTrace", false);
                    animCtrl.SetBool("isAttack", true);
                    animCtrl.SetInteger("Attack_Type", (int)Boss_State.DefaultAtt);
                }
            }
            
            yield return new WaitForSeconds(0.01f);
        }
    }

    // 이벤트 코드 부분
    #region 2p_Skill1 코드
    public void SetPos_P2Skill1()
    {
        dummy_Obj = Instantiate(Curr_BossPos_Pref, this.transform.position, Quaternion.identity);
        this.transform.position = p2_Skill1_MoonPos.position;
        this.GetComponent<Rigidbody2D>().simulated = false;
    }

    public void Attack_P2Skill1()
    {
        for (int i = 0; i < 6; i++)
        {
            float RandomData = (Random.insideUnitSphere).x;

            // -20 ~ -5
            // -35 ~ -20
            // -50 ~ -35
            // -65 ~ -50
            // -80 ~ -65
            // -95 ~ -80
            float randomAngle = Random.Range(minAngle + (i * 15), maxAngle + (i * 15));
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);
            Debug.Log(randomAngle);
            
            float randomSize = Random.Range(minSize, maxSize);
            
            // 프리팹 생성
            bladeDummy = Instantiate(BladePref, Blade_SponPos.position, randomRotation);
            bladeDummy.transform.localScale = new Vector3(randomSize, randomSize, 1);
        }

        this.GetComponent<SpriteRenderer>().enabled = false;
        Invoke("End_P2Skill1", 2f);
    }

    private void End_P2Skill1()
    {
        Invoke("Appear_Boss", 1f);
        this.transform.position = dummy_Obj.gameObject.transform.position;
        Destroy(dummy_Obj);
        
        this.GetComponent<Rigidbody2D>().simulated = true;
    }
    
    private void Appear_Boss()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
        animCtrl.SetBool("isLanding", true);
    }
    #endregion

    #region 2p_Skill2 코드
    public void SetPos_P2S2()
    {
        this.GetComponent<Rigidbody2D>().simulated = false;
        this.transform.position = new Vector2(player_pos.x, p2_Skill1_MoonPos.transform.position.y);
    }
    
    public void HitCheck_2PSkill2()
    {
        // 플레이어가 맞았다면, 모션 끝
        if (!isHit_Player_fromP2S2)
        {
            Debug.Log("맞았음");
            
            animCtrl.SetBool("isFixed", false);
            Invoke("EndSetting", 0.5f);
        }

        // 플레이어가 맞지 않았다면, 스스로 기절
        else
        {
            Debug.Log("안 맞았음");
            
            animCtrl.SetBool("isFixed", true);
        }
    }

    public void End_FixedSwordMotion()
    {
        animCtrl.SetBool("isFixed", false);
        EndAttack();
        EndSkill();
    }
    #endregion

    #region 히트 박스 생성 및 삭제
    public void On_HitArea(int index)
    {
        if (current_State == Boss_State.DefaultAtt)
        {
            float thrustValue = DA_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            if (!flapX)
            {
                if(thrustValue < 0) DA_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue * -1;
            }
            
            DA_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (current_State == Boss_State.p1_Skill1)
        {
            float thrustValue = P1Skill1_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            if (!flapX){
                if (thrustValue < 0)
                    P1Skill1_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue * -1;
            }
            else {
                if(thrustValue > 0) 
                    P1Skill1_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue * -1;
            }
            
            P1Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (current_State == Boss_State.p1_Skill2)
        {
            float thrustValue = P1Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            if (!flapX){
                if (thrustValue < 0)
                    P1Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue * -1;
            }

            else{
                if (thrustValue > 0)
                    P1Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue * -1;
            }

            P1Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (current_State == Boss_State.p2_Skill2)
        {
            float thrustValue = P2Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue;
            if (!flapX){
                if (thrustValue < 0)
                    P2Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue * -1;
            }
            else{
                if (thrustValue > 0)
                    P2Skill2_HitArea[index].gameObject.GetComponent<HitColider>().thrustValue = thrustValue * -1;
            }
            this.GetComponent<Rigidbody2D>().simulated = true;
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * -p2s2Att_force, ForceMode2D.Impulse);
            P2Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else return;
    }
    
    public void Off_HitArea(int index)
    {
        if (current_State == Boss_State.DefaultAtt)
        {
            DA_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (current_State == Boss_State.p1_Skill1)
        {
            P1Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (current_State == Boss_State.p1_Skill2)
        {
            P1Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (current_State == Boss_State.p2_Skill2)
        {
            P2Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else return;
    }
    #endregion

    #region 피격 모션 생성
    public void HitEffect()
    {
        this.gameObject.GetComponent<SpriteRenderer>().material = hit_Mat;
        Invoke("Off_Effect", 0.3f);
    }

    private void Off_Effect()
    {
        this.gameObject.GetComponent<SpriteRenderer>().material = origin_Mat;
    }
    #endregion
    
    private Boss_State Change_IntToState(int selectedBossSkill_int, ref float skillDist)
    {
        if (selectedBossSkill_int == (int)Boss_State.p1_Skill1)
        {
            skillDist = p1_Skill1_dist;
            return Boss_State.p1_Skill1;
        }
        else if (selectedBossSkill_int == (int)Boss_State.p1_Skill2)
        {
            skillDist = p1_Skill2_dist;
            return Boss_State.p1_Skill2;
        }
        else if (selectedBossSkill_int == (int)Boss_State.p2_Skill1)
        {
            skillDist = p2_Skill1_dist;
            return Boss_State.p2_Skill1;
        }
        else if (selectedBossSkill_int == (int)Boss_State.p2_Skill2)
        {
            skillDist = p2_Skill2_dist;
            return Boss_State.p2_Skill2;
        }
        else if (selectedBossSkill_int == (int)Boss_State.p2_Skill3)
        {
            skillDist = p2_Skill3_dist;
            return Boss_State.p2_Skill3;
        }
        else return 0;
    }
    
    #region 스킬 종료및 전투 종료 함수
    public void EndSkill()
    {
        Real_EndSkill();
    }

    private void Real_EndSkill()
    {
        isOnSkill = false;
        animCtrl.SetBool("isLanding", false);
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.idle);
        
        if (current_State == Boss_State.p1_Skill1)
        {
            Instantiate(GSkill_Pref, skillSpon_Pos.position, Quaternion.identity);
            if(flapX) this.transform.rotation = Quaternion.Euler(0, 180, 0);
            else this.transform.rotation = Quaternion.Euler(0, 0, 0);
            
            GSkill_Pref.GetComponent<BoxCollider2D>().enabled = true;
            GSkill_Pref.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
        }
        
        current_State = Boss_State.trace;
        animCtrl.SetBool("isAttack", false);
        animCtrl.SetBool("isTrace", true);

        isCoolTime = true;
        skill_CountTime = 0f;
    }

    public void EndAttack()
    {
        Real_EndAttack();
    }

    private void Real_EndAttack()
    {
        isAttack = false;
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.idle);
    }

    private void EndSetting()
    {
        EndSkill();
        EndAttack();
    }
    #endregion
}
