using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemInteraction : MonoBehaviour
{
    public bool isOnline = true;

    GameObject target;

    public enum ItemType
    {
        Skill, Spawn
    }
    public bool notDestroy = false;
    public ItemType item;
    public string itemName;

    public string fixedItemName;

    public GameObject effect;

    private void Start()
    {
        if (itemName == "")
        {
            StartCoroutine(SpawnItem());
        }
    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        switch (item) 
        {
            case ItemType.Skill :
                SkillManager skills = coll.GetComponent<SkillManager>();
                if (!skills) return;
                skills.AddSkill(itemName);
                break;
        }
        if (!notDestroy)
        {
            GameObject eff = Instantiate(effect);
            eff.transform.position = transform.position;
            Destroy(gameObject);
        }
    }

    IEnumerator SpawnItem() 
    {
        int value = 0;
        while (true)
        {
            yield return new WaitForSeconds(5);
            if (target == null)
            {
                yield return new WaitForSeconds(3);
                value = Random.Range(0, 51);
                if(value < 11) 
                {
                    value = Random.Range(3, 5);
                }
                else
                {
                    value = Random.Range(0, 3);
                }
                if(fixedItemName == "Desert")
                    value = 0;
                switch (value) 
                {
                    case 0:
                        itemName = "SKILL-ITEM (Gun)";
                            break;
                    case 1:
                        itemName = "SKILL-ITEM (Sword)";
                        break;
                    case 2:
                        itemName = "SKILL-ITEM (Kunai)";
                        break;
                    case 3:
                        itemName = "SKILL-ITEM (Hammer)";
                        break;
                    case 4:
                        itemName = "SKILL-ITEM (Potion)";
                        break;
                }
                target = GameObject.Instantiate(Resources.Load<GameObject>("Item/" + itemName));
                target.transform.position = transform.position;
                target.transform.rotation = Quaternion.identity;
            }
        }
    }
}
