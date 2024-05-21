using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class Blade_Ctrl : MonoBehaviour
{
    private Rigidbody2D myRd;
    private Animator myAnimCtrl;
    [SerializeField] private float forceAmount = 30f;

    public void OwnerSetting(GameObject boss)
    {
        this.gameObject.GetComponent<BladeHitColl>().owner = boss.gameObject.GetComponent<Entity>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        myRd = this.GetComponent<Rigidbody2D>();
        myAnimCtrl = this.GetComponent<Animator>();
        
        myRd.AddForce(transform.right * forceAmount, ForceMode2D.Impulse);
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.gameObject.name.Contains("Blade"))
    //         return;
    //     
    //     myRd.simulated = false;
    //     myAnimCtrl.SetTrigger("Broken");
    // }

    public void End_Broken()
    {
        Destroy(this.gameObject);
    }
}
