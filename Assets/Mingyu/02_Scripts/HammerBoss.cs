using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBoss : Boss
{
    [SerializeField] private Transform leftPlateEnd; 
    [SerializeField] private Transform rightPlateEnd;
    private Rigidbody2D myRd;
    private int nextMove = 1;
    [SerializeField] private float groundApproachDist;
    
    void Start()
    {
        base.Start();
        bossState = new Boss_State();
        Init_StateValueData(ref bossState);
        
        bossType = BossType.Hammer;
        myRd = this.gameObject.GetComponent<Rigidbody2D>();
    }
    
    protected override void Init_StateValueData(ref Boss_State state)
    {
        state.defaultAtt_dist = 1f;

        state.skill_CoolTime = 4f;
    
        state.p1_Skill1_dist = 1.5f;
        state.p1_Skill2_dist = 1.8f;
    
        state.p2_Skill1_dist = 5000f;
        state.p2_Skill2_dist = 5000f;
        state.p2_Skill3_dist = 5000f;
    }

    protected override void UpdateAnimation()
    {
        animCtrl.SetBool("isTrace", bossState.currentState == Boss_State.State.trace);
        animCtrl.SetBool("isAttack", bossState.isAttacking);
        animCtrl.SetInteger("Attack_Type", (int)bossState.currentState);

        if (bossState.currentState == Boss_State.State.trace ||
            (selectedTurn_State.Count != 0 && selectedTurn_State.Contains(bossState.currentState) &&
             !bossState.isStopTurn))
        {
            nextMove = this.transform.position.x > player_pos.x ? -1 : 1;
            myRd.velocity = new Vector2(nextMove, myRd.position.y);

            Vector2 frontVec = new Vector2(myRd.position.x + nextMove * groundApproachDist, myRd.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0,0,1));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Wall"));
            Debug.Log(rayHit.GetType());

            if (rayHit.collider != null)
            {
                
                if (rayHit.distance < 0.5f)
                {
                    Debug.Log("낭떨어지");
                    nextMove *= -1;
                }
            }

        }
    }
}
