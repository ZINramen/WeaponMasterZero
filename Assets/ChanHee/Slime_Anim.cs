using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_Anim : MonoBehaviour
{
    public int hp = 100;
    public float moveSpeed = 1.0f;
    public float jumpPower = 5.0f;
    public float attackPower = 10.0f;
    public bool isDead = false;

    private Animator animator;

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
            case State.Hit:
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
        // Move backwards
        Vector2 direction = (transform.position - player.transform.position).normalized;
        transform.Translate(direction * (moveSpeed * Time.deltaTime));
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
            player.hp -= attackPower;
            currentState = State.Hit;
        }
    }
}