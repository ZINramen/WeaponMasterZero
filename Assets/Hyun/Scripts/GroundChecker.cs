using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public Entity owner;
    bool onGround = false;
    public bool GetOnGround => onGround;

    private void Update()
    {
        if(owner && onGround == false)
            owner.aManager.ani.SetTrigger("fall");
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Camera") && !collision.gameObject.CompareTag("Boss") && !collision.gameObject.CompareTag("Particle"))
            onGround = true;

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onGround = false; 
    }
}
