using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss_Move : MonoBehaviour
{
    private GameObject player;
    private Vector2 player_pos;
    private Rigidbody myRd;

    public float move_Speed;

    private enum Boss_State
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
    
    private Animator animCtrl;

    [SerializeField] private GameObject[] DA_HitArea;
    [SerializeField] private GameObject[] P1Skill1_HitArea;
    [SerializeField] private GameObject[] P1Skill2_HitArea;
    [SerializeField] private GameObject[] P2Skill2_HitArea;
    private int index = 0;

    [SerializeField] private Transform skillSpon_Pos;
    [SerializeField] private GameObject GSkill_Pref;
    
    // Start is ca
    // lled before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").gameObject;
        
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
        this.GetComponent<SpriteRenderer>().flipX = flapX;

        if (current_State == Boss_State.trace && isAttack == false)
        {
            Vector2 velo = Vector2.zero;
            this.transform.position = Vector2.SmoothDamp( this.transform.position, player_pos, 
                ref velo, move_Speed);
        }

        if ( isCoolTime && skill_CountTime >= skill_CoolTime )
        {
            bossHP_per = ( this.GetComponent<Entity>().GetHp() ) / ( this.GetComponent<Entity>().maxHP );
            if(bossHP_per >= 0.5f)
                iBossSkill = Random.Range((int)Boss_State.p1_Skill1, (int)Boss_State.p1_Skill2 + 1);
            else
                iBossSkill = Random.Range((int)Boss_State.p2_Skill1, (int)Boss_State.p2_Skill3 + 1);

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

    public void On_HitArea(int index)
    {
        if (current_State == Boss_State.DefaultAtt)
        {
            DA_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (current_State == Boss_State.p1_Skill1)
        {
            P1Skill1_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (current_State == Boss_State.p1_Skill2)
        {
            P1Skill2_HitArea[index].gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (current_State == Boss_State.p2_Skill2)
        {
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

    public void EndSkill()
    {
        isOnSkill = false;
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.idle);
        
        if (current_State == Boss_State.p1_Skill1)
        {
            Debug.Log("SEX");
            
            Instantiate(GSkill_Pref, skillSpon_Pos.position, Quaternion.identity);
            GSkill_Pref.GetComponent<SpriteRenderer>().flipX = !flapX;
            GSkill_Pref.GetComponent<BoxCollider2D>().enabled = true;
            GSkill_Pref.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
            
            Destroy(GSkill_Pref, 1.0f);
        }
        
        current_State = Boss_State.idle;
        animCtrl.SetBool("isAttack", false);
        animCtrl.SetBool("isTrace", false);

        isCoolTime = true;
        skill_CountTime = 0f;
    }

    public void EndAttack()
    {
        isAttack = false;
        animCtrl.SetInteger("Attack_Type", (int)Boss_State.idle);
    }
}
