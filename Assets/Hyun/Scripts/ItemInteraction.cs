using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static HitColider;

public class ItemInteraction : MonoBehaviour
{
    //public bool isOnline = true;

    //GameObject target;

    //public enum ItemType
    //{
    //    Skill, Spawn
    //}
    //public bool notDestroy = false;
    //public ItemType item;
    //public string itemName;

    //public string fixedItemName;

    //public GameObject effect;

    public bool entityTarget = false;
    public bool playerTaget = false;
    public bool donHit = false;

    public UnityEvent InteractionEvent;
    public bool isEnd = false;
    public GameObject Effect;
    DynamicCamera cam;

    public Vector2 pos;
    public string itemName;

    GameObject target;

    public int EffectCode = -1;

    private void Awake()
    {
        switch (EffectCode)
        {
            case 0: // Hp Upgrade
                if (PlayerPrefs.GetInt("MaxHP-Desert", 0) != 0) 
                {
                    Destroy(gameObject);
                };
                break;
            case 1: // Sword Skill
                if (PlayerPrefs.GetInt("SwordSkill", 0) != 0)
                {
                    Destroy(gameObject);
                };
                break;
            case 2: // Gun Skill
                if (PlayerPrefs.GetInt("GunSkill", 0) != 0)
                {
                    Destroy(gameObject);
                };
                break;
            case 3: // Hammer Skill
                if (PlayerPrefs.GetInt("HammerSkill", 0) != 0)
                {
                    Destroy(gameObject);
                };
                break;
            case 4: // Hp Upgrade
                if (PlayerPrefs.GetInt("MaxHP-Ice", 0) != 0)
                {
                    Destroy(gameObject);
                };
                break;
        }
    }

    public void GetSpecialEffect(int code) 
    {
        switch (code)
        {
            case 0: // Hp Upgrade
                PlayerPrefs.SetInt("MaxHP-Desert", 20);
                Entity.Player.SetHp(Entity.Player.maxHP + PlayerPrefs.GetInt("MaxHP-Desert", 0) + PlayerPrefs.GetInt("MaxHP-Ice", 0));
                break;
            case 1: // Sword Skill
                PlayerPrefs.SetInt("SwordSkill", 1);
                break;
            case 2: // Gun Skill
                PlayerPrefs.SetInt("GunSkill", 1);
                break;
            case 3: // Hammer Skill
                PlayerPrefs.SetInt("HammerSkill", 1);
                break;
            case 4: // Hp Upgrade
                PlayerPrefs.SetInt("MaxHP-Ice", 20);
                Entity.Player.SetHp(Entity.Player.maxHP + PlayerPrefs.GetInt("MaxHP-Desert", 0) + PlayerPrefs.GetInt("MaxHP-Ice", 0));
                break;
        }
    }

    public void Jumping(float value)
    {
        Movement move = null;
        if (target)
            move = target.GetComponent<Movement>();
        if(move)
            move.Jump(value);
    }

    private void Start()
    {
        //if (itemName == "")
        //{
        //    StartCoroutine(SpawnItem());
        //}
        cam = Camera.main.GetComponent<DynamicCamera>();
    }

    private void Update()
    {
        if(!isEnd)
        if (itemName == "Hammer" && Mathf.Abs((int)transform.position.y - (int)Entity.Player.aManager.groundCheck.transform.position.y) < 4)
        {
            isEnd = true;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            GetComponent<AudioSource>().Play();
            cam.ShakeScreen(3);
            Effect.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (playerTaget && !coll.gameObject.CompareTag("Player")) return;
        if (donHit)
        {
            Entity eTarget = null;
            if (coll.GetComponent<HitColider>())
                eTarget = coll.GetComponent<HitColider>().owner;
            if (eTarget)
            {
                InteractionEvent?.Invoke();
                eTarget.SetHp(0);
            }
        }
        target = coll.gameObject;
        if (isEnd)
            return;
        if(itemName == "Stop")
        {
            Entity.Player.movement.speed = 0;
        }
        if (itemName == "PunchDestroy")
        {
            HitColider hitC =  coll.GetComponent<HitColider>();
            if (hitC == null) return;
            InteractionEvent?.Invoke();
            Instantiate(Effect, transform.position, Quaternion.identity);
            isEnd = true;
            Destroy(gameObject);
        }
        if(itemName == "Teleport")
        {
            if(coll.gameObject == Entity.Player.gameObject)
            {
                coll.transform.position = pos;
            }
        }
        if (itemName == "Event")
        {
            if (entityTarget && coll.GetComponent<Entity>() == null)
            {
                return;
            }
            InteractionEvent.Invoke();
        }
        if(itemName == "BulletParrying")
        {
            if (this.gameObject.GetComponent<BulletCtrl>())
            {
                if (coll.gameObject.GetComponent<HitColider>() &&
                    coll.gameObject.GetComponent<HitColider>().attType == AttackType.Player_SwordAtt)
                {
                    InteractionEvent?.Invoke();
                }
            }
        }
        //switch (item) 
        //{
        //    case ItemType.Skill :
        //        SkillManager skills = coll.GetComponent<SkillManager>();
        //        if (!skills) return;
        //        break;
        //}
        //if (!notDestroy)
        //{
        //    GameObject eff = Instantiate(effect);
        //    eff.transform.position = transform.position;
        //    Destroy(gameObject);
        //}
    }
    private void OnTriggerExit2D(Collider2D coll)
    {
        target = null;
    }
    //IEnumerator SpawnItem() 
    //{
    //    int value = 0;
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(5);
    //        if (target == null)
    //        {
    //            yield return new WaitForSeconds(3);
    //            value = Random.Range(0, 51);
    //            if(value < 11) 
    //            {
    //                value = Random.Range(3, 5);
    //            }
    //            else
    //            {
    //                value = Random.Range(0, 3);
    //            }
    //            if(fixedItemName == "Desert")
    //                value = 0;
    //            switch (value) 
    //            {
    //                case 0:
    //                    itemName = "SKILL-ITEM (Gun)";
    //                        break;
    //                case 1:
    //                    itemName = "SKILL-ITEM (Sword)";
    //                    break;
    //                case 2:
    //                    itemName = "SKILL-ITEM (Kunai)";
    //                    break;
    //                case 3:
    //                    itemName = "SKILL-ITEM (Hammer)";
    //                    break;
    //                case 4:
    //                    itemName = "SKILL-ITEM (Potion)";
    //                    break;
    //            }
    //            target = GameObject.Instantiate(Resources.Load<GameObject>("Item/" + itemName));
    //            target.transform.position = transform.position;
    //            target.transform.rotation = Quaternion.identity;
    //        }
    //    }
    //}
}
