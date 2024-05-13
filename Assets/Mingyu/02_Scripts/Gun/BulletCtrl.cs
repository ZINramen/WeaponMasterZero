using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float install_ZValue;
    public enum BulletType
    {
        General = 0,
        Parring,
        Piecing
    }

    private BulletType bulletType;

    public BulletType Get_BulletType()
    {
        return bulletType;
    }
    
    private Vector3 shootingDir = new Vector3(0, 0, 0);
    
    private Rigidbody2D myRd;
    [SerializeField] private float addForce = 5f;
    
    private void Start()
    {
        shootingDir.z = install_ZValue;
        Debug.Log(shootingDir.z);
        
        myRd = this.gameObject.GetComponent<Rigidbody2D>();
        this.transform.rotation = Quaternion.Euler(shootingDir);
        
        myRd.AddForce(this.transform.right * addForce, ForceMode2D.Impulse);
        
        Invoke("BrokenBullet", 2f);
    }

    public void BrokenBullet()
    {
        Destroy(this.gameObject);
    }
}
