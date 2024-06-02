using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using static UnityEditor.PlayerSettings;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    bool end = false;
    public bool collDestroy = false;
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
                    StartCoroutine("SpawnObjectAndDestroy");
                }
                else if (transform.position.x > maxValue || transform.position.y > maxValue)
                {
                    StartCoroutine("SpawnObjectAndDestroy");
                }
                else if (transform.position.x < -maxValue || transform.position.y < -maxValue)
                {
                    StartCoroutine("SpawnObjectAndDestroy");
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

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (!end)
        if (collDestroy)
        {
            Entity et = collision.gameObject.GetComponent<Entity>();
            if (et && et.gameObject.tag != "Player")
            {
                if (DestroyBeforeSpawnObject != null)
                    Instantiate(DestroyBeforeSpawnObject, transform.position, Quaternion.identity);
                end = true;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!end)
        if (collDestroy)
        {
            Entity et = other.gameObject.GetComponent<Entity>();
            if (et && et.gameObject.tag != "Player")
            {  
                if (DestroyBeforeSpawnObject != null)
                    Instantiate(DestroyBeforeSpawnObject, transform.position, Quaternion.identity);
                end = true;
                Destroy(gameObject);
            }
        }
    }
}
