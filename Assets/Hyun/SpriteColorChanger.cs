
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorChanger : MonoBehaviour
{
    SpriteRenderer render;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        render.color = new Color(10, 10, 10, 1);
    }
}
