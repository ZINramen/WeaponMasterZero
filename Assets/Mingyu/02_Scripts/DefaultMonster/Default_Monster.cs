using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Default_MonsterState
{
    public enum State 
    { 
        idle = 0,
        trace = 1,
        DefaultAtt = 2
    }
    public State currentState;

    public float traceDistance;
    public float defaultAtt_dist;

    public bool isAttacking = false;
    public bool isStopTurn = false;
}

public abstract class Default_Monster : MonoBehaviour
{
    protected GameObject player;
    public Vector2 player_pos;
    
    protected float nextMove = 1;
    protected bool isMove;
    public float move_Speed;
    private bool isEndSetting = false;

    protected GameObject AttHitCol;

    protected float stopDelayTime;
    
    protected RaycastHit2D rayHit;
    
    protected Rigidbody2D myRd;
    protected Animator animCtrl;
    
    protected Default_MonsterState monsterState;
    protected bool isMoveEnd = false;
    
    [SerializeField] protected float distFrom_Player;
    public bool isDie;

    protected void Start()
    {
        GameObject[] playerTagObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerTagObj in playerTagObjects)
        {
            if (playerTagObj.name == "APO")
                player = playerTagObj;
        }
        
        myRd = this.gameObject.GetComponent<Movement>().GetBody();
        
        Move(-move_Speed, -1);
        
        if (player != null)
        {
            player_pos = player.GetComponent<Transform>().position;
        }

        animCtrl = GetComponent<Animator>();
    }
    
    protected abstract void Init_StateValueData(ref Default_MonsterState state);
    
    protected void Update()
    {
        Debug.Log(monsterState.currentState);
        
        if (this.gameObject.GetComponent<Entity>().GetHp() > 0)
        {
            UpdateState();
            UpdateAnimation();
            UpdateSeeting();
        }

        if (isMoveEnd)
        {
            //StartCoroutine(StopMove());
            isMoveEnd = false;
        }
    }

    protected virtual void UpdateSeeting() { }
    
    protected IEnumerator StopMove()
    {
        while (myRd.velocity.magnitude > 0.5f)
        {
            yield return new WaitForSeconds(stopDelayTime);
            myRd.velocity = Vector2.zero;
        }
    }
    
    public void MoveXPos_Mingyu(float x)
    {
        if (rayHit.collider == null)
            x = 0;
        
        int plus = 1;
        if (transform.localEulerAngles.y == 180) plus = -1;
        myRd.AddForce(new Vector2(x * 100 * plus, 0));

        isMoveEnd = true;
    }

    protected virtual float EachBossMoveSetting(RaycastHit2D rayHit, float x)
    {
        if (rayHit.collider == null)
            return 0f;

        return x;
    }
    
    private void UpdateState()
    {
        player_pos = player.GetComponent<Transform>().position;
        distFrom_Player = Mathf.Abs(player_pos.x - transform.position.x);
        
        // 상황에 따른 동작 구현 FSM
        if (distFrom_Player >= monsterState.traceDistance)
        {
            monsterState.currentState = Default_MonsterState.State.idle;
            Move(0,1);
        }
        
        // 공격중이 아니거나 && 맞는중이 아니라면
        else if (!monsterState.isAttacking)
        {
            if (distFrom_Player >= monsterState.defaultAtt_dist)
            {
                monsterState.currentState = Default_MonsterState.State.trace;
            }
            else
            {
                myRd.velocity = Vector2.zero;
                
                monsterState.currentState = Default_MonsterState.State.DefaultAtt;
                monsterState.isAttacking = true;
            }
        }
    }

    private void UpdateAnimation()
    {
        if (monsterState.isAttacking == false)
            animCtrl.SetBool("isTrace", monsterState.currentState == Default_MonsterState.State.trace);
        else
            animCtrl.SetBool("isTrace", false);
        
        animCtrl.SetBool("isAttack", monsterState.isAttacking);
        
        if (monsterState.currentState == Default_MonsterState.State.trace)
        {
            Debug.Log("Move");
            this.transform.rotation = Quaternion.Euler(0, this.transform.position.x > player_pos.x ? 0 : 180, 0);
            MoveSetting();
        }
    }

    public virtual void HitMotion()
    {
        animCtrl.SetTrigger("Hit");
    }
    
    protected virtual void MoveSetting()
    {
        isMove = true;
        nextMove = this.transform.position.x > player_pos.x ? -move_Speed : move_Speed;
        
        Move(nextMove, nextMove > 0 ? 1 : -1);
    }

    public void Check_AttackHitCol(int isOnColider)
    {
        if (AttHitCol == null)
            return;
        
        DefaultMonster_OnColliderType(AttHitCol, isOnColider);
    }
    
    private void DefaultMonster_OnColliderType(GameObject colliderObj, int i_CheckOnColier)
    {
        bool isOnColider;
        if (i_CheckOnColier == 1) isOnColider = true;
        else isOnColider = false;
        
        if (colliderObj.GetComponent<BoxCollider2D>() != null)
        {
            colliderObj.gameObject.GetComponent<BoxCollider2D>().enabled = isOnColider;
        }
        else if (colliderObj.GetComponent<CircleCollider2D>() != null)
        {
            colliderObj.gameObject.GetComponent<CircleCollider2D>().enabled = isOnColider;
        }
        else if (colliderObj.GetComponent<EdgeCollider2D>() != null)
        {
            colliderObj.gameObject.GetComponent<EdgeCollider2D>().enabled = isOnColider;
        }
        else
            return;
    }
    
    protected void Move(float inputNextMove, int turnValue)
    {
        if (this.gameObject.GetComponent<Entity>().isDamaged) return;
        
        myRd.velocity = new Vector2(inputNextMove, myRd.position.y);

        Vector2 frontVec = new Vector2(myRd.position.x, myRd.position.y - 0.5f);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 0, 1));      // #Test용
        
        rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));
    }
    
    public void EndAttack()
    {
        monsterState.currentState = Default_MonsterState.State.idle;
        Debug.Log("EndAtt");
        isMove = false;
        
        animCtrl.SetBool("isAttack", false);
        
        monsterState.isStopTurn = false;
        monsterState.isAttacking = false;
    }
    
    private void EndSetting()
    {
        isEndSetting = false;
    }
}