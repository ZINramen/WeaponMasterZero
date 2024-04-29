using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadCtrl : MonoBehaviour
{
    private float moveSpeed;
    private Animator anim;
    private SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        StartCoroutine(ToadMove());
    }

    private void Update()
    {
        transform.Translate(moveSpeed, 0, 0);
    }

    IEnumerator ToadMove()
    {
        while (true)
        {
            anim.SetBool("isWalk", false);
            moveSpeed = 0;

            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));

            anim.SetBool("isWalk", true);
            moveSpeed = Random.Range(-0.015f, 0.015f);
            rend.flipX = moveSpeed > 0 ? false : true;

            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        }
    }
}
