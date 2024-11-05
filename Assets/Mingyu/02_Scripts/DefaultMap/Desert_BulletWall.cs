using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desert_BulletWall : MonoBehaviour
{
    private Transform[] redBulletSponPos_List;
    [SerializeField] private GameObject redBullet_Pref;
    [SerializeField] private GameObject dummyMonster;

    private bool isAttack = false;
    private float attackCount = 0f;
    private float attack_Delay = 2f;

    private GameObject DummyRed;

    private void Start()
    {
        redBulletSponPos_List = new Transform[3];
        
        redBulletSponPos_List[0] = this.transform.GetChild(0).gameObject.transform;
        redBulletSponPos_List[1] = this.transform.GetChild(1).gameObject.transform;
        redBulletSponPos_List[2] = this.transform.GetChild(2).gameObject.transform;
    }

    private void Update()
    {
        if (isAttack)
        {
            attackCount += Time.deltaTime;

            if (attackCount >= attack_Delay)
            {
                Attack();
                attackCount = 0f;
            }
        }
    }

    // 플레이어가 하트를 먹으면 발동
    public void isAttack_On()
    {
        isAttack = true;
    }

    public void isAttack_Off()
    {
        isAttack = false;
    }

    private void Attack()
    {
        for (int i = 0; i < 3; i++)
        {
            
            DummyRed = Instantiate(redBullet_Pref,
                redBulletSponPos_List[i].position, Quaternion.identity);

            DummyRed.gameObject.GetComponent<HitColider>().owner 
                = dummyMonster.gameObject.GetComponent<Entity>();
        }
    }
}
