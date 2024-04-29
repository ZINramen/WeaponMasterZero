using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Venogear2DPlatformer
{
    public class PlayerMovement : MonoBehaviour
    {

        public CharacterController2D controller;
        public Animator animator;
        public float runSpeed = 40f;
        public float attackCooldown = 0.3f;

        float horizontalMove = 0f;

        private bool attackBlocked;
        bool jump = false;
        bool crouch = false;

        // Update is called once per frame
        private void Update()
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetBool("IsWalking", horizontalMove != 0);

            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }

            if (Input.GetButtonDown("Crouch"))
            {
                crouch = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                crouch = false;
            }

            if (controller.m_Grounded)
            {
                animator.SetBool("IsClimb", false);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                PlayerAttack();
            }
        }

        private void PlayerAttack()
        {
            if (attackBlocked)
                return;

            animator.SetTrigger("Attack");
            attackBlocked = true;
            StartCoroutine(DelayAttack());
        }

        private IEnumerator DelayAttack()
        {
            yield return new WaitForSeconds(attackCooldown);
            attackBlocked = false;
        }

        public void OnLanding()
        {
            animator.SetBool("IsJumping", false);
        }

        public void OnCrouching(bool IsCrouching)
        {
            animator.SetBool("IsCrouch", IsCrouching);
        }

        private void FixedUpdate()
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }
    }
}