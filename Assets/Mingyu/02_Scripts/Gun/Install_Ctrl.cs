using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Bullet_Type
{
    up = 0,
    right = 1,
    down = 2,
    left = 3
}

public class Install_Ctrl : MonoBehaviour
{
    private Animator myAnim;
    public bool isActive = false;
    public Entity BossObj;

    private float attack_Delay = 0.5f;
    private float attackCount = 0f;

    private GameObject[] BulletSpon_PosArr;
    [SerializeField] private GameObject General_Bullet_Pref;
    private Vector3 BulletSpon_Pos;
    private Bullet_Type bulletType;
    
    private GameObject dummy_GeneralBullet;
    private int MaxHP = 9999;
    public bool isDefaultMap = false;

    public void SetMaxHP(int input_MaxHP)
    {
        MaxHP = input_MaxHP;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        myAnim = this.gameObject.GetComponent<Animator>();
        InitValue();
        
        this.gameObject.SetActive(false);
        
    }

    private void InitValue()
    {
        attack_Delay = 2.5f;

        BulletSpon_PosArr = new GameObject[4];
        BulletSpon_PosArr[0] = gameObject.transform.GetChild(0).gameObject;
        BulletSpon_PosArr[1] = gameObject.transform.GetChild(1).gameObject;
        BulletSpon_PosArr[2] = gameObject.transform.GetChild(2).gameObject;
        BulletSpon_PosArr[3] = gameObject.transform.GetChild(3).gameObject;
    }

    private void Update()
    {
        if (isActive)
        {
            myAnim.SetBool("is_Attack", false);
            attackCount += Time.deltaTime;

            if (attackCount >= attack_Delay)
            {
                Attack();
                attackCount = 0f;
            }
        }

        Entity thisEntity = this.gameObject.GetComponent<Entity>();
        if (thisEntity && thisEntity.GetHp() != MaxHP)
        {
            myAnim.SetTrigger("Hit");
        }
        myAnim.SetBool("isActive", isActive);
    }

    private void Attack()
    {
        float currAngle = this.transform.rotation.eulerAngles.z;
        
        for (int i = 0; i < 4; i++)
        {   
            myAnim.SetBool("is_Attack", true);
            SponBullet(i);
        }
    }

    private void SponBullet(int index)
    {
        BulletSpon_Pos = BulletSpon_PosArr[index].gameObject.GetComponent<Transform>().position;
        
        dummy_GeneralBullet = Instantiate(General_Bullet_Pref, BulletSpon_Pos, Quaternion.identity);
        dummy_GeneralBullet.GetComponent<BulletCtrl>().install_ZValue = (this.transform.rotation.eulerAngles.z) - (90 * index);
        
        dummy_GeneralBullet.gameObject.GetComponent<HitColider>().owner = BossObj;
    }

    public void EndSetting()
    {
        BossObj.gameObject.GetComponent<GunBoss>().Broken_InstallObj(this.gameObject);
    }
}
