using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Entity owner;
    int currentLeftWeapon = 2;
    int currentRightWeapon = 3;

    public GameObject healEffect;
    public HUDController hudControl;

    public int currentWeapon = 0;
    public int haveWeaponNum = 0;
    public bool infinite = false;
    bool isHealthEvent = false;
    private void Start()
    {
        owner = GetComponent<Entity>();

        if (hudControl)
            hudControl.ChangeCurrentWeapon(currentWeapon);

        ChangeWeaponSkill(false, currentWeapon);
        CheckMPUI();
    }

    private void Update()
    {
        if (owner)
            owner.aManager.ani.SetInteger("Weapon", currentWeapon);
        
        if (hudControl)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangeWeaponSkill(true);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                ChangeWeaponSkill(false);
            }
            if (Input.GetKeyDown(KeyCode.S)) // 체력 회복
            {
                int curGauge = owner.aManager.ani.GetInteger("Gauge");
                if (curGauge > 0)
                {
                    Instantiate(healEffect, owner.transform.position, Quaternion.identity).transform.parent = owner.transform;
                    owner.SetHp(owner.GetHp() + owner.maxHP * 0.3f); // 체력 회복 퍼센테이지 0.3
                    isHealthEvent = true;
                    ReduceGauge();
                }
            }
        }
        if (owner.GetMp() == owner.maxMp) 
        {
            int curGauge = owner.aManager.ani.GetInteger("Gauge");
            if (curGauge < 3)
            {
                owner.aManager.ani.SetInteger("Gauge", curGauge + 1);
                CheckMPUI();
            }
            owner.SetMp(0);
        }
    }


    public void ChangeWeaponSkill(bool isLeftWeapon, int newWeapon = -1) 
    {
        var previousWeapon = currentWeapon;
        if (newWeapon == -1)
        {
            if (isLeftWeapon)
                currentWeapon = currentLeftWeapon;
            else
                currentWeapon = currentRightWeapon;
        }
        else
            currentWeapon = newWeapon;

        if(haveWeaponNum < currentWeapon) 
        {
            currentWeapon = previousWeapon;
            return;
        }

        hudControl.ChangeCurrentWeapon(currentWeapon);

        bool left = false;
        for (int i = 0; i < 3; i++)
        {
            if (i != (currentWeapon - 1))
            {
                if (!left)
                {
                    left = true;
                    currentLeftWeapon = i + 1;
                }
                else
                    currentRightWeapon = i + 1;
            }
        }
    }

    public void CheckMPUI()
    {
        int curGauge = owner.aManager.ani.GetInteger("Gauge");
        for (int i = 0; i < hudControl.GaugeIcons.Length; i++)
        {
            hudControl.GaugeIcons[i].SetActive(false);
        }
        for (int i = 0; i < curGauge; i++)
        {
            hudControl.GaugeIcons[i].SetActive(true);
        }
    }

    public void ReduceGauge() 
    {
        int curGauge = owner.aManager.ani.GetInteger("Gauge");
        if (curGauge == 3 && !isHealthEvent)
        {
            owner.aManager.ani.SetInteger("Gauge", 0);
            for (int i = 0; i < hudControl.GaugeIcons.Length; i++)
            {
                hudControl.GaugeIcons[i].SetActive(false);
            }
        }
        else
        {
            owner.aManager.ani.SetInteger("Gauge", curGauge - 1);
            hudControl.GaugeIcons[curGauge - 1].SetActive(false);
        }
        isHealthEvent = false;
    }
}
