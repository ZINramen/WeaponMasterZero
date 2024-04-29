using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Venogear2DPlatformer
{
    public class LandAi : MonoBehaviour
    {
        public Animator m_Animator;
        public Transform target;
        public float moveSpeed;
        public float fleeSpeed;
        public float distance;
        public float Attackdistance = 1;
        public bool AttackOrFlee;

        private bool startMoving;
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = gameObject.GetComponent<Animator>();
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            target = go.transform;
        }

        void FixedUpdate()
        {
            distance = Vector2.Distance(transform.position, target.transform.position);

            // Flip sprite if chase
            if (target.position.x < transform.position.x && AttackOrFlee == true)
            {
                spriteRenderer.flipX = true;
                //    Debug.Log("Chase, target on left side");
            }
            if (target.position.x < transform.position.x && AttackOrFlee == false && startMoving)
            {
                spriteRenderer.flipX = false;
                //    Debug.Log("Flee, target on left side");
            }

            // Flip sprite if flee
            if (target.position.x > transform.position.x && AttackOrFlee == true)
            {
                spriteRenderer.flipX = false;
                //   Debug.Log("Chase, target on right side");
            }
            if (target.position.x > transform.position.x && AttackOrFlee == false && startMoving)
            {
                spriteRenderer.flipX = true;
                //   Debug.Log("Flee, target on right side");
            }

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
                StartMove();
            }
        }

        void Update()
        {
            // Move to attack
            if (distance > 6 && startMoving == true)
            {
                AttackOrFlee = true;
            }

            // Set attack trigger
            if (distance < Attackdistance && AttackOrFlee == true)
            {
                m_Animator.SetTrigger("Attack");
            }

            // Reset attack trigger
            if (distance > 1 && AttackOrFlee == false)
            {
                m_Animator.ResetTrigger("Attack");
            }
        }

        void StartMove()
        {
            startMoving = true;
            m_Animator.SetBool("Move", true);
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