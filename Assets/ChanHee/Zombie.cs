using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float detectionRange = 5f; // 플레이어 탐지 범위
    public float attackRange = 1f; // 공격 범위
    public float attackDelay = 0.5f; // 공격 딜레이
    public float stunDuration = 1f; // 스턴 지속 시간
    public GameObject exclamationPointPrefab; // 느낌표 Prefab

    private GameObject player; // 플레이어 참조
    private Animator animator; // 애니메이터 참조

    private enum State
    {
        Idle,
        Chase,
        Attack,
        Hit
    }

    private State currentState;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        currentState = State.Idle;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Hit:
                Hit();
                break;
        }
    }

    private void Idle()
    {
        // 플레이어가 탐지 범위 내에 있는지 확인
        if (Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            // 느낌표 Prefab 인스턴스화
            Instantiate(exclamationPointPrefab, transform.position + Vector3.up, Quaternion.identity);
            // Chase 상태로 전환
            currentState = State.Chase;
        }
    }

    private void Chase()
    {
        // 플레이어를 향해 이동
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * Time.deltaTime);

        // 플레이어가 공격 범위 내에 있는지 확인
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            // Attack 상태로 전환
            currentState = State.Attack;
        }
    }

    private void Attack()
    {
        // 공격 애니메이션 재생
        animator.SetTrigger("Attack");

        // 공격 딜레이 후 Chase 상태로 전환
        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        currentState = State.Chase;
    }

    private void Hit()
    {
        // 스턴 애니메이션 재생
        animator.SetTrigger("Hit");

        // 스턴 지속 시간 후 Idle 상태로 전환
        StartCoroutine(StunDuration());
    }

    private IEnumerator StunDuration()
    {
        yield return new WaitForSeconds(stunDuration);
        currentState = State.Idle;
    }
}