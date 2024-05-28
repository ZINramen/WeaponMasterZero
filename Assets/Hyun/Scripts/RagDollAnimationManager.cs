using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollAnimationManager : MonoBehaviour
{
    public Entity owner;
    int jump = 0, maxJump = 2;
    
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(owner.movement.h != 0)
            ani.SetBool("isWalk", true);
        else
            ani.SetBool("isWalk", false);

        if (owner.aManager.groundCheck.GetOnGround)
        {
            ani.SetBool("isFloating", false);
            jump = 0;
        }
        else
        {
            if (jump == 0) jump++;
            ani.SetBool("isFloating", true);
        }
        if (jump < maxJump && Input.GetKeyDown(owner.aManager.Jump))
        {
            if (jump != 0)
            {
                GameObject eff = GameObject.Instantiate(owner.aManager.Ec.jumpDustEffectPrefab);
                eff.transform.position = gameObject.transform.position - new Vector3(0, 0.5f, 0);
                jump++;
            }
            owner.movement.Jump(0);
            ani.SetBool("isFloating", true);
        }
    }
}
