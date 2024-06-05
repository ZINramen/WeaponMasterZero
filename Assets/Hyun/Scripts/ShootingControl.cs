using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShootingControl : MonoBehaviour
{
    Vector3 AtargetingPos;
    public bool WhenTargeting = true;
    public Texture2D targetingImage;

    public GameObject gun;
    public Animator gunAnimator;
    public GameObject bullet;
    public float bulletSpeed = 10;

    public GameObject shootingPrefab;
    public GameObject bombPrefab;
    public GameObject HammerPrefab;
    public GameObject StonePrefab;
    public GameObject ShockWavePrefab;

    public Transform fireTr;
    public Transform GunfireTr;
    Entity owner;

    public Image GunGauge;

    int bulletCount = 0;
    public int bulletMaxCount = 6;

    // Start is called before the first frame update
    void Start()
    {
        owner = transform.GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!owner.isDie)
        if (WhenTargeting)
        {
            Cursor.SetCursor(targetingImage, Vector2.zero, CursorMode.Auto);
            AtargetingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(20, -20, 0));

            var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            var angle = MathF.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            
            gun.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
            gunAnimator.transform.localEulerAngles = new Vector3(gunAnimator.transform.localEulerAngles.x, owner.transform.localEulerAngles.y, gunAnimator.transform.localEulerAngles.z);

            if (bulletCount < bulletMaxCount)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    bulletCount++;
                    if (GunGauge.fillAmount > 0.8f)
                    {
                        GunGauge.fillAmount -= 0.15f;
                    }
                    else
                    {
                        GunGauge.fillAmount -= 0.17f;
                    }
                    var obj = Instantiate(bullet, fireTr.position, Quaternion.identity);
                    obj.GetComponent<HitColider>().owner = owner;
                    Destroyer d = obj.GetComponent<Destroyer>();
                    obj.transform.LookAt(AtargetingPos);
                    d.moveSpeed = bulletSpeed;
                    d.haveTarget = true;
                    if (gunAnimator != null)
                    {
                        gunAnimator.SetTrigger("Shoot");
                    }
                    owner.aManager.ResetAttackTriggerEvent();
                }
                if (bulletCount == bulletMaxCount)
                    StartCoroutine(Reload());
            }
        }
        else
        {
            if (GunGauge)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                bulletCount = 0;
                GunGauge.fillAmount = 1.0f;
            }
        }
    }

    public void SpawnHammer()
    {
        Instantiate(HammerPrefab, transform.position + transform.right * 3 + new Vector3(0, 15), Quaternion.identity);
    }

    public void RandomShooting(float shotArea)
    {
        var targetingPos = owner.transform.position + new Vector3(UnityEngine.Random.Range(-shotArea, shotArea), UnityEngine.Random.Range(-shotArea, shotArea));
        Instantiate(shootingPrefab, targetingPos, Quaternion.identity).GetComponent<HitColider>().owner = owner;
    }

    public void SimpleShoot(float speed)
    {
        Vector2 len = this.gameObject.GetComponent<Movement>().mousePos - this.gameObject.transform.position;
        float z = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;
        
        var obj = Instantiate(bullet, GunfireTr.position, Quaternion.Euler(0f, 0f, z));
        obj.GetComponent<HitColider>().owner = owner;
        Destroyer d = obj.GetComponent<Destroyer>();
        d.moveSpeed = speed;
    }
    public void ThrowBomb()
    {
        var obj = Instantiate(bombPrefab, fireTr.position, Quaternion.identity);
        Destroyer d = obj.GetComponent<Destroyer>();
        obj.transform.LookAt(AtargetingPos);
        d.moveSpeed = bulletSpeed;
        d.haveTarget = true;
    }


    public void ThrowStone()
    {
        float tempPosX;
        if (owner.transform.rotation.y == 0) tempPosX = 1.5f;
        else tempPosX = -1.5f;
        
        var stone = Instantiate(StonePrefab, owner.transform.position + new Vector3(tempPosX, 0.3f),
            Quaternion.identity);
        stone.GetComponent<Rigidbody2D>().AddForce(owner.transform.right * 10000);
    }
    
    public void ShockWave()
    {
        float tempPosX;
        if (owner.transform.rotation.y == 0) tempPosX = 0.5f;
        else tempPosX = -0.5f;
        
        var shockWave = Instantiate(ShockWavePrefab, owner.transform.position + new Vector3(tempPosX, -0.5f),
            Quaternion.identity);
        shockWave.GetComponent<HitColider>().owner = owner;
        shockWave.GetComponent<Rigidbody2D>().AddForce(owner.transform.right * 10000);
    }
    
    IEnumerator Reload()
    {
        while (GunGauge.fillAmount != 1)
        {
            yield return new WaitForSeconds(0.05f);
            GunGauge.fillAmount += 0.05f;
        }
        bulletCount = 0;
    }
}
