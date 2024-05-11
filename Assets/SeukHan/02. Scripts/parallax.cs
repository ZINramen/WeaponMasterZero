using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class parallax : MonoBehaviour
{
    private Material mat;
    private float distance;

    [Range(0f, 0.5f)] public float speed = 0.2f;
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        distance += Time.deltaTime * speed;
        mat.SetTextureOffset("_MainTex", Vector3.right * distance);
    }
}
