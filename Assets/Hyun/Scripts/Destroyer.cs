using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float moveSpeed = 0;
    public bool fromParent;
    public float delayTime = 0;

    public GameObject DestroyBeforeSpawnObject;

    private void Start()
    {
        if (moveSpeed != 0)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.right * moveSpeed;
        }
    }
    void Update()
    {
        if (fromParent)
        {
            transform.parent = null;
        }
        else
        {
            StartCoroutine(SpawnObject());
            Destroy(gameObject, delayTime);
        }
    }
    IEnumerator SpawnObject() 
    {
        yield return new WaitForSeconds(delayTime-0.1f);

        if (DestroyBeforeSpawnObject != null)
            Instantiate(DestroyBeforeSpawnObject, transform.position, Quaternion.identity);
    }
}
