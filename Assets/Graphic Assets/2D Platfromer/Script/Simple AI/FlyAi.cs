using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Venogear2DPlatformer
{
    public class FlyAi : MonoBehaviour
    {
        public Animator m_Animator;
        public Transform target;
        public float moveSpeed;
        public float fleeSpeed;
        public float distance;
        public bool AttackOrFlee;

        private bool startMoving;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D this_Rigidbody;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            this_Rigidbody = GetComponent<Rigidbody2D>();
            m_Animator = gameObject.GetComponent<Animator>();
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            target = go.transform;
        }

        void FixedUpdate()
        {
            distance = Vector2.Distance(transform.position, target.transform.position);

            // Flip sprite if chase
            if (target.position.x > transform.position.x && AttackOrFlee == true)
            { spriteRenderer.flipX = false; }
            if (target.position.x < transform.position.x && AttackOrFlee == true)
            { spriteRenderer.flipX = true; }

            // Flip sprite if flee
            if (target.position.x < transform.position.x && AttackOrFlee == false)
            { spriteRenderer.flipX = false; }
            if (target.position.x > transform.position.x && AttackOrFlee == false)
            { spriteRenderer.flipX = true; }

            // Change state
            if (distance < 0.8)
            {
                AttackOrFlee = false;
            }

            // Start idle_A animation
            if (distance > 4f && startMoving == false)
            {
                m_Animator.SetBool("Player nearby", false);
            }

            // Start idle_B animation
            if (distance < 6f && startMoving == false)
            {
                m_Animator.SetBool("Player nearby", true);
            }

            // Start moving (Chase or Flee)
            if (startMoving == true || distance < 2)
            {
                if (AttackOrFlee)
                    Chase();
                else
                {
                    Flee();
                }

                if (startMoving) { return; }
                StartFly();
            }
        }

        void Update()
        {
            // Move to attack
            if (distance > 5 && startMoving == true)
            {
                AttackOrFlee = true;
            }

            // Set attack trigger
            if (distance < 1.5 && AttackOrFlee == true)
            {
                m_Animator.SetTrigger("Attack");
            }

            // Reset attack trigger
            if (distance > 1 && AttackOrFlee == false)
            {
                m_Animator.ResetTrigger("Attack");
            }
        }

        void StartFly()
        {
            this_Rigidbody.constraints = RigidbodyConstraints2D.None;
            this_Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            startMoving = true;
            m_Animator.SetBool("Fly", true);
        }

        void Chase()
        {
            Vector2 direction = target.transform.position - transform.position;
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        }

        void Flee()
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), -moveSpeed * Time.deltaTime);
            Vector3 dir = transform.position - target.position;
            transform.Translate(fleeSpeed * Time.fixedDeltaTime * dir);
        }

    }
}