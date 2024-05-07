using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Anim : Entity
{
    // Hp는 Entity에 포함되어 있어 삭제함.
    Entity player;
    public float moveSpeed = 1.0f;
    public float jumpPower = 5.0f;
    public float attackPower = 10.0f;
    public bool isDead = false;

    private Animator animator;
    private AnimationManager ani_manager; // 애니메이션 재생 및 애니메이션에 특수 효과 추가해주는 관리자

    private enum State
    {
        Idle,
        Chase,
        Attack,
        Hit
    }

    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Idle;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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
            case State.Hit: // CurrentState에서 Hit로 바꾸는 기준 : Entity의 isDamaged 변수 값이 True이면 데미지를 받고 있는 거임.
                Hit();
                break;
        }
    }

    private void Idle()
    {
        // Move in a random direction
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        Vector2 direction = new Vector2(x, y).normalized;
        transform.Translate(direction * (moveSpeed * Time.deltaTime));
        animator.Play("Idle");
    }

    private void Chase()
    {
        // Find the direction to the player
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * (moveSpeed * Time.deltaTime));
        animator.Play("Chase");
    }

    private void Attack()
    {
        // Jump towards the player
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * (jumpPower * Time.deltaTime));
        animator.Play("Attack");
    }

    private void Hit() 
    {
        /* Move backwards => Entity의 Damaged 함수에 구현된 내용임. 해당 내용은 자동으로 호출되니 No 터치 가능.
        Vector2 direction = (transform.position - player.transform.position).normalized;
        transform.Translate(direction * (moveSpeed * Time.deltaTime));
        */        
        animator.Play("Hit");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            currentState = State.Attack;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Decrease the player's health
            player.Damaged(attackPower);
            currentState = State.Hit;
        }
    }
}