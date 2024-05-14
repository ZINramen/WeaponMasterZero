using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystem : MonoBehaviour
{
    bool waitAttack = false;
    bool eventEnd = false;
    Entity owner;

    GameObject player;

    public float AttackDelay = 0.5f;
    public string WalkName; // 걷는 애니메이션 - 불 파라미터 이름. 빈칸이면 애니메이션 미존재
    public string AttackName; // 공격 애니메이션 - 트리거 파라미터 이름. 빈칸이면 애니메이션 미존재

    void Awake()
    {
        player = GameObject.FindWithTag("Player");    
    }
    
    void Start()
    {
        owner = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        // ai의 주인이 존재하는 경우만 실행
        if (!owner) return;

        // 공격 상황 체크해 공격 시작
        AIAttack();
    }

    protected virtual void AIAttack() // 자식 클래스에서 수정 가능함.
    {
        // 디폴트 = 근처에 플레이어가 있을 시 공격
        nearbyAttack(3, AttackDelay);
    }
    public virtual void AIMove() // 자식 클래스에서 수정 가능함.
    {
        // 디폴트 = 플레이어를 추격만 함.
        Chase();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 아래부터는 이미 만들어져있는 기능 : 재사용 가능함.
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Ai_Acts
    void Chase()
    {
        Movement move = owner.movement;
        Animator am =  owner.aManager.ani;
        if (Mathf.Abs(owner.ai.player.transform.position.x - transform.position.x) < 1)
        {
            move.h = 0;
            move.body.velocity = Vector2.zero;
            if (WalkName != "")
            {
                am.SetBool(WalkName, false);
            }
            return;
        }

        if (owner.ai.player.transform.position.x < transform.position.x)
            move.h = -1;
        else if (owner.ai.player.transform.position.x > transform.position.x)
            owner.movement.h = 1;

        move.body.velocity = new Vector2(move.h * 100 * move.speed * Time.deltaTime, move.body.velocity.y);

        if (move.h != 0)
        {
            if (WalkName != "")
                am.SetBool(WalkName, true);
            if (move.h < 0)
                transform.localEulerAngles = new Vector3(0, 180, 0);
            else
                transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (WalkName != "")
            am.SetBool(WalkName, false);
    }
    void nearbyAttack(float attackArea, float delay) 
    {
        // 가까울 때 공격 실행
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < attackArea)
        {
            if (!waitAttack)
                StartCoroutine(DelayAttack(attackArea, delay));
        }
        else
        {
            if (AttackName != "")
            {
                owner.aManager.ani.ResetTrigger(AttackName);
            }
        }
    }
    IEnumerator DelayAttack(float attackArea, float dtime)
    {
        waitAttack = true;
        yield return new WaitForSeconds(dtime);
        if (AttackName != "" && (Mathf.Abs(player.transform.position.x - transform.position.x) < attackArea))
            owner.aManager.ani.SetTrigger(AttackName);
        waitAttack = false;
    }
    #endregion
}
