using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignCtrl : MonoBehaviour
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
        dummyShootEffect = Instantiate(ShootEffect, this.transform.position, Quaternion.identity);
        dummyShootEffect.transform.parent = this.transform;
        
        myAnim.SetBool("isShoot", true);
        myCollider.enabled = true;
    }
}
