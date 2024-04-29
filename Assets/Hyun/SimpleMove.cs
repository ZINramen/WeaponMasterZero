using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    Rigidbody2D r;
    public enum Direction { Up, Right }
    public Direction currentDir = Direction.Right;
    public float power = 0;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (currentDir == Direction.Right)
            r.AddForce(transform.right * power);
        else if (currentDir == Direction.Up)
            r.AddForce(transform.up * power);
    }
}
