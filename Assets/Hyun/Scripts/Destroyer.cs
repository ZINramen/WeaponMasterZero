using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using static UnityEditor.PlayerSettings;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public bool haveDestroyArea = false;
    public bool haveTarget = false;

    public float moveSpeed = 0;
    public bool fromParent;
    public bool NoDestroyButHide = false;
    public float delayTime = 0;

    public float maxValue = 10;

    public GameObject DestroyBeforeSpawnObject;

    bool startDestroy = false;
    private void Start()
    {
        if (moveSpeed != 0)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (haveTarget)
            {
                Vector2 pos = transform.forward;
                pos.Normalize();
                
                rb.velocity = new Vector3(pos.x, pos.y, 0) * moveSpeed;
                
            }
            else
            {
                rb.velocity = transform.right * moveSpeed;
            }
            transform.rotation = Quaternion.identity;
        }
    }
    void Update()
    {
        if (!startDestroy)
        {
            if (fromParent)
            {
                transform.parent = null;
                startDestroy = true;
            }
            else
            {
                if (!haveDestroyArea)
                {
                    StartCoroutine(SpawnObjectAndDestroy());
                }
                else if (transform.position.x > maxValue || transform.position.y > maxValue)
                {
                    StartCoroutine(SpawnObjectAndDestroy());
                }
                else if (transform.position.x < -maxValue || transform.position.y < -maxValue)
                {
                    StartCoroutine(SpawnObjectAndDestroy());
                }
            }
        }
    }
    IEnumerator SpawnObjectAndDestroy()
    {
        startDestroy = true;
        yield return new WaitForSeconds(delayTime);
        if (DestroyBeforeSpawnObject != null)
            Instantiate(DestroyBeforeSpawnObject, transform.position, Quaternion.identity);
        if (!NoDestroyButHide)
            Destroy(gameObject);
        else
        {
            startDestroy = false;
            gameObject.SetActive(false);
        }
    }
}
