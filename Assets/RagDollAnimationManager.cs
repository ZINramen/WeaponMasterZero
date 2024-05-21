using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollAnimationManager : MonoBehaviour
{
    public Entity owner;
    ShootingControl sc;
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

        if(owner.aManager.groundCheck.GetOnGround)
            ani.SetBool("isFloating", false);
        else
            ani.SetBool("isFloating", true);

        if (Input.GetKeyDown(owner.aManager.Jump))
        {
            owner.movement.Jump(0);
        }
    }
}
