using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Destroyer : MonoBehaviour
{
    public bool haveTarget = false;
    public float moveSpeed = 0;
    public bool fromParent;
    public float delayTime = 0;

    public GameObject DestroyBeforeSpawnObject;

    private void Start()
    {
        if (moveSpeed != 0)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (haveTarget)
            {
                Vector3 pos = transform.forward;
                pos = new Vector3(pos.x, pos.y, 0);
                pos.Normalize();
                rb.velocity = pos * moveSpeed;
            }
            else
            {
                rb.velocity = transform.right * moveSpeed;
            }
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
