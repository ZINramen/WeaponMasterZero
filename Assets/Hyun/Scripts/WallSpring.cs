using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpring : MonoBehaviour
{
    public int valueX, valueY;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(valueX, valueY));
        }
    }
}
