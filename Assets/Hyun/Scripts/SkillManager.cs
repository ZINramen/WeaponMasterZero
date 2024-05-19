using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Entity owner;
    int currentLeftWeapon = 2;
    int currentRightWeapon = 3;

    public HUDController hudControl;
    public int currentWeapon = 0;
    public int haveWeaponNum = 0;
    public bool infinite = false;
    private void Start()
    {
        owner = GetComponent<Entity>();

        if (hudControl)
            hudControl.ChangeCurrentWeapon(currentWeapon);

        ChangeWeaponSkill(false, currentWeapon);
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
        }
        if (owner.GetMp() == owner.maxMp) 
        {
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
}
