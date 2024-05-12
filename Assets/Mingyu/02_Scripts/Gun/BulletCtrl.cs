using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    private Animator myAnim;
    private CircleCollider2D myCollider;
    
    [SerializeField] private GameObject ShootEffect;
    private GameObject dummyShootEffect;
    
    private void Start()
    {
        myAnim = this.gameObject.GetComponent<Animator>();
        myCollider = this.gameObject.GetComponent<CircleCollider2D>();
        myCollider.enabled = false;
    }

    public void Shoot_Bullet()
    {
        dummyShootEffect = Instantiate(ShootEffect, this.transform.position, quaternion.identity);
        dummyShootEffect.transform.parent = this.transform;
        
        myAnim.SetBool("isShoot", true);
        myCollider.enabled = true;
    }
}
