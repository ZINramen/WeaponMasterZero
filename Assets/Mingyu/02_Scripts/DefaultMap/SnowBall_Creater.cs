using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SnowBall_Creater : MonoBehaviour
{
    [SerializeField] private GameObject SnowBallPref;
    private GameObject DummySnowBall;
    
    private float attackCount = 0;
    private float attack_Delay = 7f;
    private float attPower = 100000f;
    
    private void Update()
    {
        attackCount += Time.deltaTime;

        if (attackCount >= attack_Delay)
        {
            Attack();
            attackCount = 0f;
        }
    }

    private void Attack()
    {
        DummySnowBall = GameObject.Instantiate(SnowBallPref
            , this.gameObject.transform.position
            , quaternion.identity);

        DummySnowBall.transform.GetChild(0).GetComponent<SnowBall_HitColl>().isDefaultMap = true;
        DummySnowBall.transform.GetChild(0).GetComponent<SnowBall_HitColl>().rollingPower = -150f;
        DummySnowBall.GetComponent<Rigidbody2D>().AddForce(Vector2.left * attPower, ForceMode2D.Impulse);
        
    }
}
