using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Move : MonoBehaviour
{
    private GameObject player;
    private Vector2 player_pos;
    private Rigidbody myRd;
    
    public float move_Speed;
    
    private enum Boss_State { idle = -2, trace, DefaultAtt = 0, p1_Skill1 = 1, p1_Skill2 = 2, p2_Skill1 = 3, p2_Skill2 = 4, p2_skill3 = 5 }

    private int trace_Dist = 5000;
    [SerializeField] private float defaultAtt_dist = 1.3f;
    
    [SerializeField] private float p1_Skill1_dist = 2f;
    [SerializeField] private float p1_skill2_dist = 2.5f;
    
    private float p2_Skill1 = 5000f;
    private float p2_skill2 = 5000f;
    private float p2_skill3 = 5000f;

    private Boss_State current_State;
    
    private float bossHP_per = 1.0f;
    [SerializeField] private const float skill_CoolTime = 2.0f;
    private float skill_CountTime = 2.0f;
    private bool isCoolTime = false;
    private int selectes_BossSkill;
    
    private Animator animCtrl;
    
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
        
        Vector2 velo = Vector2.zero;
        this.transform.position = Vector2.SmoothDamp( this.transform.position, player_pos, 
            ref velo, move_Speed);

        if (isCoolTime)
        {
            skill_CountTime += Time.deltaTime;
        }
    }

    IEnumerator Check_MonsterState()
    {
        while (true)
        {
            bossHP_per = ( this.GetComponent<Entity>().GetHp() ) / ( this.GetComponent<Entity>().GetHp() );
            if(bossHP_per >= 0.5f)
                selectes_BossSkill = Random.Range((int)Boss_State.p1_Skill1, 3);
            else
                selectes_BossSkill = Random.Range((int)Boss_State.p2_Skill1, (int)Boss_State.p2_skill3);
            
            float distFrom_Player = Vector2.Distance(player_pos, this.transform.position);

            if (distFrom_Player >= trace_Dist)
            {
                current_State = Boss_State.idle;
                animCtrl.SetBool("isTrace", false);
                animCtrl.SetBool("isAttack", false);
            }
            
            else if (distFrom_Player <= 2.0f && skill_CountTime >= skill_CoolTime)
            {
                current_State = Boss_State.p1_Skill1;
                isCoolTime = true;
                skill_CountTime = 0f;
                
                animCtrl.SetBool("isAttack", true);
                animCtrl.SetInteger("Attack_Type", selectes_BossSkill);
            }
            
            else if (distFrom_Player >= defaultAtt_dist)
            {
                current_State = Boss_State.trace;
                
                animCtrl.SetBool("isTrace", true);
                animCtrl.SetBool("isAttack", false);
            }
            else
            {
                current_State = Boss_State.DefaultAtt;
                animCtrl.SetBool("isAttack", true);
                animCtrl.SetInteger("Attack_Type", (int)Boss_State.DefaultAtt);
            }
            
            Debug.Log(current_State);
            
            Boss_Act();
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Boss_Act()
    {
        
    }
}
