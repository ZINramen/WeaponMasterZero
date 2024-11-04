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
    public GameObject bullet_Super;

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

    float bulletEnergy = 0;
    int bulletCount = 0;
    bool bulletEnergyFull = false;

    public int bulletMaxCount = 6;

    // Start is called before the first frame update
    void Start()
    {
        owner = transform.GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        if(owner.movement.PlayerType)
        if(!owner.isDie)
        if (WhenTargeting) // 플레이어가 총을 무기로 사용하고 있으며 사격 가능한 상태
        {
            Cursor.SetCursor(targetingImage, new Vector2(targetingImage.width/3f, targetingImage.height / 3f), CursorMode.Auto); // 커서를 사격 조준경으로 바꿈.
            AtargetingPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(20, -20, 0)); // 마우스 위치를 변수에 저장함.

            // gun은 총 오브젝트를 의미하며 아래 변수들은 마우스의 방향에 따라 총의 회전값이 변하도록 하였다.
            var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            var angle = MathF.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            
            gun.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);

            // 총을 사용 가능한 상태에선 gunAnimator라는 애니메이터를 이용해 총의 발사 애니메이션을 보여준다.
            // 아래는 gunAnimator를 가진 게임 오브젝트의 회전 값을 바꾸어 총의 위치가 제대로 위치하도록 하는 코드.
            gunAnimator.transform.localEulerAngles = new Vector3(gunAnimator.transform.localEulerAngles.x, owner.transform.localEulerAngles.y, gunAnimator.transform.localEulerAngles.z);

            if (bulletCount < bulletMaxCount) // 총의 발사 횟수 제한이 있고, 해당 횟수를 넘어서면 일정 시간 동안 재장전이 이뤄진다.
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (bulletEnergy <= 0.3f)
                        bulletEnergy += Time.deltaTime;
                    else if(!bulletEnergyFull)
                    {
                        bulletEnergyFull = true;
                        gunAnimator.SetTrigger("Full");
                    }
                }
                if (Input.GetKeyUp(KeyCode.Mouse0)) 
                {    
                    GameObject bulletTemp = bullet;
                    if (bulletEnergy > 0.3f)
                    {
                        bullet = bullet_Super;
                        GetComponent<AudioSource>().PlayOneShot(Resources.Load("Bullet Super") as AudioClip);
                    }
                    else 
                    {
                                GetComponent<AudioSource>().PlayOneShot(Resources.Load("Bullet") as AudioClip);
                            }
                    // 아래는 총을 발사하는 과정을 나타낸 코드이다.

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

                    bulletEnergyFull = false;
                    bulletEnergy = 0;
                    bullet = bulletTemp;
                }
                if (bulletCount == bulletMaxCount)
                    StartCoroutine(Reload()); // 재장전을 담당하는 코루틴 호출
            }
        }
        else
        {
            if (GunGauge)
            {
                // 총알 에너지 초기화
                bulletEnergyFull = false;
                bulletEnergy = 0;

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
        // 총알이 마우스 방향을 향하도록 하는 코드
        Vector2 len = this.gameObject.GetComponent<Movement>().mousePos - this.gameObject.transform.position;
        float z = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;
        var obj = Instantiate(bullet, GunfireTr.position, Quaternion.Euler(0f, 0f, z));

        // 총알의 속도를 조절하고, 플레이어는 해당 총알에 맞지 않도록 설정함.
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
        // 바위를 생성해 앞쪽으로 날린다.
        var stone = Instantiate(StonePrefab, owner.transform.position + new Vector3(tempPosX, 0.3f),
            Quaternion.identity);
        stone.GetComponent<Rigidbody2D>().AddForce(owner.transform.right * 10000);

        // 바위는 플레이어가 맞지 않도록 만든 오브젝트라 별도 처리가 필요 없다.
    }
    
    public void ShockWave()
    {
        float tempPosX;
        if (owner.transform.rotation.y == 0) tempPosX = 0.5f;
        else tempPosX = -0.5f;

        // 참격을 생성해 앞쪽으로 날린다.
        var shockWave = Instantiate(ShockWavePrefab, owner.transform.position + new Vector3(tempPosX, -0.5f),
            Quaternion.identity);

        // 참격의 경우 플레이어가 맞지 않게 설정함.
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
