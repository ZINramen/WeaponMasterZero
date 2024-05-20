using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Entity owner;
    public int currentWeapon = 0;
    public string[] skills = new string[6];
    public bool infinite = false;
    private void Start()
    {
        owner = GetComponent<Entity>();
        if(owner)
            owner.aManager.ani.SetInteger("Weapon", currentWeapon);
    }

    private void Update()
    {
        if(owner.GetMp() == owner.maxMp) 
        {
            if(skills[1] == "Sword")
                skills[5] = "Ult2";
            else
                skills[5] = "Ult1";
        }
        else 
        {
            skills[5] = "";
        }
    }

    public void AddSkill(string name)
    {
        ModifySkill(name, name);
    }

    public void ModifySkill(string name, string newName) 
    {
        if(name == "Gun") 
        {
            skills[0] = newName;


        }
        if (name == "Sword")
        {
            skills[1] = newName;

        }
        if (name == "Kunai")
        {
            skills[2] = newName;

        }
        if (name == "Hammer")
        {
            skills[3] = newName;

        }
        if (name == "Potion")
        {
            skills[4] = newName;

        }
    }
    public void RemoveSKill(string value) 
    {
        if (infinite) return;
        ModifySkill(value, "");
    }


}
