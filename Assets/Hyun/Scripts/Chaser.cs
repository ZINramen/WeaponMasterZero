using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : MonoBehaviour
{
    SpriteRenderer sp;
    bool isChase = false;
    bool death = false;

    [SerializeField]
    bool playerIsTarget = false;
    public float delayTime = 3.0f;

    [SerializeField]
    protected bool isMove = false;
    protected UnityEngine.Vector3 destination;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        if (playerIsTarget)
        {
            StartCoroutine(ChaseTaget(Entity.Player, delayTime));
        }
    }
    private void Update()
    {
        if (isMove)
        {
            GoToDestination();
        }
    }

    protected void GoToDestination()
    {
        if(sp != null)
        {
            if (destination.x > transform.position.x)
                sp.flipX = true;
            else
                sp.flipX = false;
        }
        transform.position = Vector3.Lerp(transform.position, destination, 1 * Time.deltaTime);
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            isMove = false;
        }
    }
    protected IEnumerator ChaseTaget(Entity target, float delay)
    {
        while (!death)
        {
            yield return new WaitForSeconds(delay);
            if (!isMove)
            {
                isMove = true;
                destination = target.transform.position;
            }
        }
    }

}
