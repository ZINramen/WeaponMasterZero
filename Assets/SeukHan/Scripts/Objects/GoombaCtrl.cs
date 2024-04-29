using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GoombaCtrl : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private SpawnGoomba sGoomba;

    private Vector3 FlipY = new Vector3(0.0f, 180.0f, 0.0f);

    [SerializeField]
    private float moveX = -3.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        sGoomba = FindObjectOfType<SpawnGoomba>();
        sGoomba.GoombaCount++;
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject temp = coll.gameObject;

        if (temp.tag != "Terrain")
        {
            if (temp.tag == "Pipe")
            {
                moveX *= -1;
                transform.Rotate(FlipY);
                return;
            }

            if (temp.layer == LayerMask.NameToLayer("Entity"))
            {
                if (temp.GetComponent<Rigidbody2D>().velocity.y < 0 && temp.transform.position.y > transform.position.y + 0.2)
                {
                    StartCoroutine(GoombaDied());
                    temp.GetComponent<Entity>().movement.Jump(2);
                }
                else
                {
                    if (transform.localEulerAngles.y != 0)
                        temp.GetComponent<Entity>().Damaged(10, 10);
                    else
                        temp.GetComponent<Entity>().Damaged(10, -10);
                }
            }

            StartCoroutine(GoombaDied());
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(moveX, rb.velocity.y);
    }

    IEnumerator GoombaDied()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(3.0f);
        sGoomba.GoombaCount--;
        Destroy(gameObject);
    }
}
