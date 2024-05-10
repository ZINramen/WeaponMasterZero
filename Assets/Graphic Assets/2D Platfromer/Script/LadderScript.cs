using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Venogear2DPlatformer
{
    public class LadderScript : MonoBehaviour
    {
        public float VerticalSpeed = 5;
        private Rigidbody2D rb2D;

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (rb2D == null) { rb2D = other.GetComponent<Rigidbody2D>(); }

                if (Input.GetKey(KeyCode.W))
                {
                    // Climb Up
                    rb2D.velocity = new Vector2(rb2D.velocity.x, VerticalSpeed);
                    SetClimbingAnimation(other, true, true, 1f);
                }

                else if (Input.GetKey(KeyCode.S))
                {
                    // Climb Down
                    rb2D.velocity = new Vector2(rb2D.velocity.x, -VerticalSpeed);
                    SetClimbingAnimation(other, true, true, 1f);
                }

                else if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    // Stay on ladder
                    rb2D.gravityScale = 0;
                    rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
                    SetClimbingAnimation(other, false, true, 0);
                }
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other is CircleCollider2D)
            {
                //Debug.Log("exit collider");
                rb2D = other.GetComponent<Rigidbody2D>();
                rb2D.gravityScale = 3;
                SetClimbingAnimation(other, false, false, 0);
            }
        }

        void SetClimbingAnimation(Collider2D other, bool isClimbing, bool isOnLadder, float climbSpeed)
        {
            Animator animator = other.GetComponentInChildren<Animator>();
            animator.SetBool("IsClimb", isClimbing);
            animator.SetBool("IsOnLadder", isOnLadder);
            animator.SetFloat("ClimbSpeed", climbSpeed);
        }
    }
}