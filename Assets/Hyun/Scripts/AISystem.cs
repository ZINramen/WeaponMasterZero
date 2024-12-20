using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystem : MonoBehaviour
{
    public bool waitAttack = false;
    public bool eventEnd = false;
    public Entity owner;

    public GameObject player;

    public float detectionRange = 5f;
    public float AttackDelay = 0.5f;
    public string WalkName; // 걷는 애니메이션 - 불 파라미터 이름. 빈칸이면 애니메이션 미존재
    public string AttackName; // 공격 애니메이션 - 트리거 파라미터 이름. 빈칸이면 애니메이션 미존재
    public GameObject exclamationPointPrefab;
    protected bool exclamationPointShown = false;
    protected GameObject exclamationPointInstance;
    protected float originalSpeed;
    public bool enableSpeedControl = false;
    public Movement movement;
    public float attackRange = 1f;
    public NewLongRangeAttack newLongRangeAttack; // NewLongRangeAttack 참조

    
    void Awake()
    {
        player = GameObject.FindWithTag("Player");    
    }
    
    void Start()
    {
        owner = GetComponent<Entity>();
        originalSpeed = owner.movement.speed;
        movement = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        // ai의 주인이 존재하는 경우만 실행
        if (!owner) return;

        // 공격 상황 체크해 공격 시작
        AIAttack();
        // 이동 상황 체크해 이동 시작
        AIMove();
        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // 플레이어와의 거리가 detectionRange 이하이면 애니메이션 실행
        if (distanceToPlayer <= detectionRange && !exclamationPointShown)
        {
            exclamationPointShown = true; // 애니메이션이 한 번 실행되었음을 표시합니다.
        }
        
        if (exclamationPointInstance != null)
        {
            // 느낌표 인스턴스의 위치를 캐릭터의 머리 위로 설정합니다.
            exclamationPointInstance.transform.position = transform.position + new Vector3(0, 1, 0);
        }
    }

    protected virtual void AIAttack() // 자식 클래스에서 수정 가능함.
    {
        // 디폴트 = 근처에 플레이어가 있을 시 공격
        nearbyAttack(attackRange, AttackDelay);
    }
    public virtual void AIMove() // 자식 클래스에서 수정 가능함.
    {
        // 디폴트 = 플레이어를 추격만 함.
        Chase();
    }
    public void ToggleLongRangeAttack(bool isActive)
    {
        if (newLongRangeAttack != null)
        {
            newLongRangeAttack.enabled = isActive;
        }
    }
    
    void Chase()
    {
        Movement move = owner.movement;
        Animator am =  owner.aManager.ani;

        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // 플레이어와의 거리가 detectionRange 이상이면 이동 멈춤
        if (distanceToPlayer > detectionRange)
        {
            move.h = 0;
            if (WalkName != "")
            {
                am.SetBool(WalkName, false);
            }
            return;
        }
        else
        {
            if (enableSpeedControl)
            {
                movement.speed = originalSpeed;
            }
        }

        // 플레이어와의 거리가 1 미만일 때 이동 멈춤
        if (distanceToPlayer < 1)
        {
            move.h = 0;
            if (WalkName != "")
            {
                am.SetBool(WalkName, false);
            }
            return;
        }
        if (Mathf.Abs(owner.ai.player.transform.position.x - transform.position.x) < 1)
        {
            move.h = 0;
            if (WalkName != "")
            {
                am.SetBool(WalkName, false);
            }
            return;
        }
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < detectionRange && !exclamationPointShown)
        {
            // 느낌표 프리팹 인스턴스화 코드 (Exclamation Point Prefab)
            if (exclamationPointPrefab)
            {
                exclamationPointInstance = Instantiate(exclamationPointPrefab, transform.position, Quaternion.identity);
            }
            exclamationPointShown = true; // 느낌표가 표시되었음을 표시합니다.
        }

        // 플레이어가 탐지 범위 밖에 있고 느낌표가 표시되어 있다면 느낌표 인스턴스를 제거하고 표시 플래그를 리셋합니다.
        if (Mathf.Abs(player.transform.position.x - transform.position.x) > detectionRange && exclamationPointShown)
        {
            if (exclamationPointPrefab)
            {
                Destroy(exclamationPointInstance);
            }
            exclamationPointShown = false;
        }


        if (!move.StopMove)
        {
            if (owner.ai.player.transform.position.x < transform.position.x)
                move.h = -1;
            else if (owner.ai.player.transform.position.x > transform.position.x)
                owner.movement.h = 1;
            move.body.velocity = new Vector2(move.h * move.speed, move.body.velocity.y);
        }
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
        if (Mathf.Abs(player.transform.position.x - transform.position.x) < attackRange)
        {
            if (!waitAttack)
            {
                // 공격이 시작되기 전에 speed를 0으로 설정합니다.
                movement.speed = 0;
                StartCoroutine(DelayAttack(attackArea, delay));
            }
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
        {
            owner.aManager.ani.SetTrigger(AttackName);
            if (newLongRangeAttack != null && newLongRangeAttack.enabled)
            {
                newLongRangeAttack.FireProjectile(); // 투사체 발사
            }
        }
        waitAttack = false;
        
        movement.speed = originalSpeed;
    }
    
}

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 아래부터는 이미 만들어져있는 기능 : 재사용 가능함.
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
