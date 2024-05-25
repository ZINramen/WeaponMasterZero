using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 10; // 미끄러짐 방지를 위해 drag 값을 증가시킵니다.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

