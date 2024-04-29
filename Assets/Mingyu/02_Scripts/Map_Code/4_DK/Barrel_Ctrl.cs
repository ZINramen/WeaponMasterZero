using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel_Ctrl : MonoBehaviour
{
    private Rigidbody2D myRd;

    private const   float delet_Time = 4.0f;
    private         float countTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        myRd = this.GetComponent<Rigidbody2D>();

        myRd.AddForce(Vector2.right * 3 + Vector2.up * 2, ForceMode2D.Impulse);
    }

    private void Update()
    {
        countTime += Time.deltaTime;
        Debug.Log(countTime);

        if (countTime >= delet_Time)
            Destroy(this.gameObject);
    }
}
