using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    bool onGround = false;
    public bool GetOnGround => onGround;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Camera") && !collision.gameObject.CompareTag("Particle"))
            onGround = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onGround = false;
    }
}
