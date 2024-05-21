using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ShootingControl : MonoBehaviour
{
    public bool WhenTargeting = true;
    public Texture2D targetingImage;

    public GameObject gun;
    public Animator gunAnimator;
    public GameObject bullet;
    public float bulletSpeed = 10;

    public GameObject shootingPrefab;
    public GameObject bombPrefab;

    public Transform fireTr;
    public Transform GunfireTr;

    Entity owner;

    // Start is called before the first frame update
    void Start()
    {
        owner = transform.root.GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if (WhenTargeting)
        {
            Cursor.SetCursor(targetingImage, Vector2.zero, CursorMode.Auto);
            var targetingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(20, -20, 0));
            gun.transform.LookAt(targetingPos);

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                var obj = Instantiate(bullet, fireTr.position, Quaternion.identity);
                obj.GetComponent<HitColider>().owner = owner;
                Destroyer d = obj.GetComponent<Destroyer>();
                obj.transform.LookAt(targetingPos);
                d.moveSpeed = bulletSpeed;
                d.haveTarget = true;
                if(gunAnimator != null)
                {
                    gunAnimator.SetTrigger("Shoot");
                }
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void RandomShooting(float shotArea)
    {
        var targetingPos = owner.transform.position + new Vector3(Random.Range(-shotArea, shotArea), Random.Range(-shotArea, -shotArea / 2));
        Instantiate(shootingPrefab, targetingPos, Quaternion.identity).GetComponent<HitColider>().owner = owner;
    }

    public void SimpleShoot(float speed)
    {
        var obj = Instantiate(bullet, GunfireTr.position, transform.rotation);
        obj.GetComponent<HitColider>().owner = owner;
        Destroyer d = obj.GetComponent<Destroyer>();
        d.moveSpeed = speed;
    }
    public void ThrowBomb()
    {
        var bomb = Instantiate(bombPrefab, owner.transform.position + new Vector3(0,1), Quaternion.identity);
        bomb.GetComponent<Rigidbody2D>().AddForce((owner.transform.right + owner.transform.up) * 100);
    }
}
