using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PipeCtrl : MonoBehaviour
{
    public Vector2 playerCheckRange;
    public LayerMask playerLayer;
    public Transform spawnTransform;

    private void FixedUpdate()
    {
        playerCollCheck();
    }

    private void playerCollCheck()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position, playerCheckRange, 0, playerLayer);

        if(hit && hit.gameObject.GetComponent<Animator>().GetBool("Down"))
        {
            playerTP(hit.gameObject);
        }
    }

    private void playerTP(GameObject obj)
    {
        obj.transform.position = spawnTransform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, playerCheckRange);
    }
}
