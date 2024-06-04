using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushHitColl : MonoBehaviour
{
    [SerializeField] private float XPower;
    [SerializeField] private float YPower;

    private int checkNumber = 0;
    private float delayCount = 0;

    public bool isRush = false;

    private void Update()
    {
        if (isRush)
        {
            delayCount += Time.deltaTime;

            if (delayCount >= 0.1f)
            {
                this.gameObject.GetComponent<BoxCollider2D>().enabled = checkNumber % 2 == 0 ? true : false;
                delayCount = 0;
                checkNumber++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "APO")
        {
            other.gameObject.GetComponent<Animator>().SetTrigger("Hit");
            if(this.gameObject.transform.parent.localRotation.y == 180)
                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * XPower, ForceMode2D.Impulse);
            else
                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * XPower, ForceMode2D.Impulse);

            other.gameObject.GetComponent<Movement>().Jump(YPower);
        }
    }
}
