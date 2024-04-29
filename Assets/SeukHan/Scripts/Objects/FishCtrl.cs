using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCtrl : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    public GameObject[] ignoreColl;

    [SerializeField]
    private float moveX = -3.0f;
    private float dirChangeTime = 13.0f;

    private Vector3 jumpForce = Vector3.up * 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        foreach (GameObject obj in ignoreColl)
        {
            if (obj != null)
            {
                Physics2D.IgnoreCollision(obj.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }

        StartCoroutine(ChangeDir());
        StartCoroutine(RandomJump());
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject temp = coll.gameObject;

        if (temp.layer == LayerMask.NameToLayer("Entity")) temp.GetComponent<Entity>().Damaged(10, 5);
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(moveX, rb.velocity.y);
    }

    private void FixedUpdate()
    {
        //Vector2 v = rb.velocity;
        //var angle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        //rb.MoveRotation(angle);
    }

    IEnumerator ChangeDir()
    {
        var wait = new WaitForSeconds(dirChangeTime);
        var flip = new Vector3(0, 180.0f, 0);

        while (true)
        {
            yield return wait;
            moveX *= -1;
            transform.Rotate(flip);
        }
    }

    IEnumerator RandomJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(20.0f, 25.0f));
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
        }
    }
}
