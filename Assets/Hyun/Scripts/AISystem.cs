using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystem : MonoBehaviour
{
    bool eventEnd = false;
    Entity owner;
    public GameObject player;
    public string AttackName;

    public GameObject Enemy;
    public GameObject Enemy2;
    public GameObject Enemy3;

    // Start is called before the first frame update
    void Start()
    {
        owner = GetComponent<Entity>();
        if(Enemy)
            StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        if (owner && owner.GetHp() == 0 && !eventEnd)
        {
            eventEnd = true;
            ResultManager.KillCount += 1;
            Destroy(gameObject, 10);
        }
        if (owner && player.GetComponent<Animator>().GetBool("Down") == true)
            owner.aManager.ani.SetBool("Down", true);
        else if(owner)
            owner.aManager.ani.SetBool("Down", false);
        if (!owner) return;
        if (owner.aManager.GetOnGround() && Mathf.Abs(player.transform.position.x - transform.position.x) < 2)
        {
            if (owner.aManager.ani.GetBool("Down"))
                owner.aManager.ani.SetTrigger("Kick");
            else if (AttackName == "Hammer")
            {
                if (Random.Range(0, 2) == 0)
                {
                    owner.aManager.ani.SetTrigger("Hammer");
                }
                else 
                {
                    owner.aManager.ani.SetTrigger("Punch");
                }
            }
            else
                owner.aManager.ani.SetTrigger(AttackName);
        }
        else
        {
            owner.aManager.ani.ResetTrigger(AttackName);
            owner.aManager.ani.ResetTrigger("Punch");
        }
    }

    IEnumerator SpawnEnemy() 
    {
        while (true) 
        { 
            yield return new WaitForSeconds(5);
            if (Enemy) 
            {
                GameObject spawnedEnemy;
                if (Random.Range(0, 10) != 0)
                {
                    spawnedEnemy = Instantiate(Enemy);
                }
                else
                {
                    if (ResultManager.KillCount > 40 && Random.Range(0, 10) > 5)
                        spawnedEnemy = Instantiate(Enemy3);
                    else
                    {
                        if (ResultManager.KillCount > 20)
                            spawnedEnemy = Instantiate(Enemy2);
                        else
                            spawnedEnemy = Instantiate(Enemy);
                    }
                }
                spawnedEnemy.transform.position = transform.position;

            }
        }
    }
}
