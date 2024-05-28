using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Key_Ctrl : MonoBehaviour
{
    public GameObject Player;
    public GameObject LastBoss;

    private void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, Player.transform.position, Time.deltaTime);

        if (Vector2.Distance(this.transform.position, Player.transform.position) <= 0.2f)
            transform.position = Player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<Entity>())
        {
            LastBoss.gameObject.GetComponent<LastBoss_Ctrl>().Destory_LockObj();
            Destroy(this.gameObject);
        }
    }
}
