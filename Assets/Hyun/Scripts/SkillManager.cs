using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    Entity owner;
    public string[] skills = new string[6];
    public bool infinite = false;
    private void Start()
    {
        owner = GetComponent<Entity>();
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
            if (owner.network)
            {
                NetworkProcess(newName, 0);
            }

        }
        if (name == "Sword")
        {
            skills[1] = newName;
            if (owner.network)
            {
                NetworkProcess(newName, 1);
            }
        }
        if (name == "Kunai")
        {
            skills[2] = newName;
            if (owner.network)
            {
                NetworkProcess(newName, 2);
            }
        }
        if (name == "Hammer")
        {
            skills[3] = newName;
            if (owner.network)
            {
                NetworkProcess(newName, 3);
            }
        }
        if (name == "Potion")
        {
            skills[4] = newName;
            if (owner.network)
            {
                NetworkProcess(newName, 4);
            }
        }
    }
    public void RemoveSKill(string value) 
    {
        if (infinite) return;
        ModifySkill(value, "");
    }

    void NetworkProcess(string newName, int value)
    {
        if (newName != "")
        {
            owner.network.SkillActive[value] = true;
            owner.network.SkillIconChange(value, true);
        }
        else
        {
            owner.network.SkillActive[value] = false;
            owner.network.SkillIconChange(value, false);
        }
    }
}
