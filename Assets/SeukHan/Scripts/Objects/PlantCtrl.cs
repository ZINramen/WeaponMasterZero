using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCtrl : MonoBehaviour
{
    public Vector2 playerCheckRange;
    public LayerMask playerLayer;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        playerCollCheck();
    }

    private void playerCollCheck()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, playerCheckRange, 0, playerLayer);

        if (hits.Length > 0)
        {
            anim.SetBool("isPlayerOn", true);
        }
        else
        {
            anim.SetBool("isPlayerOn", false);
        }

    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject temp = coll.gameObject;

        if (temp.layer == LayerMask.NameToLayer("Entity"))
        {
            temp.GetComponent<Entity>().Damaged(10, 10);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, playerCheckRange);
    }

}
