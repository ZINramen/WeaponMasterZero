using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCastle_Ctrl : MonoBehaviour
{
    public GameObject Morning_Sky;
    public GameObject SunSet_Sky;

    public GameObject Monster;
    public GameObject LaserController;

    public bool         Is_MonsterAttack;
    private float       count_LaserTime;
    private const float wait_LaserTime = 2.0f;

    private void Start()
    {
        Morning_Sky.SetActive(true);
        SunSet_Sky.SetActive(false);

        Monster.SetActive(false);
    }

    public void Update()
    {
        if(Is_MonsterAttack)
        {
            Morning_Sky.SetActive(false);
            SunSet_Sky.SetActive(true);

            count_LaserTime += Time.deltaTime;

            if(count_LaserTime >= wait_LaserTime)
            {
                Monster.SetActive(true);
                LaserController.gameObject.GetComponent<LaserController>().
                    is_ShootLaser = true;
            }
        }

        else
        {
            SunSet_Sky.SetActive(false);
            Monster.SetActive(false);

            Morning_Sky.SetActive(true);
            count_LaserTime = 0;
        }
    }

    public void EndLaser_Setting()
    {
        StartCoroutine(Wait(1f));

        LaserController.gameObject.GetComponent<LaserController>().
                    is_ShootLaser = false;

        Is_MonsterAttack = false;
    }

    private IEnumerator Wait(float waitTme)
    {
        yield return waitTme;
    }
}
