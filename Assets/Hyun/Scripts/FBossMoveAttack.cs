using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
//using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

public class FBossMoveAttack : Chaser
{
    public int phase = 2; // 페이즈 선택
    // EnergyBall max number
    int CurrentEnergyBallCount = 0;
    public int MaxCurrentEnergyBallCount = 20;
    public GameObject[] EnergyBalls;

    float delayMoveTime = 5f;
    float delayMoveTime_origin = 5f;

    bool isStop = false;

    Entity owner;

    public Transform MoveTr;
    public GameObject EnergyBall;
    public GameObject Minion;
    public GameObject Fist;

    [SerializeField]
    Transform[] MoveTrs; // 이동과 관련된 트랜스폼
    Animator ani;
    
    public List<LineRenderer> lasers;
    public List<GameObject> WarningBox;

    void Start()
    {
        delayMoveTime_origin = delayMoveTime;
        ani = GetComponent<Animator>();
        MoveTrs = new Transform[MoveTr.childCount];
        owner = GetComponent<Entity>();

        if (phase == 2)
        {
            for (int i = 0; i < MoveTrs.Length; i++)
            {
                MoveTrs[i] = MoveTr.GetChild(i);
            }
            StartCoroutine(MovePos());
        }

        EnergyBalls = new GameObject[MaxCurrentEnergyBallCount];
    }

    void Update()
    {
        if (owner.isDie)
            return;
        if(owner.GetHp() < 500 && !Fist.activeSelf)
        {
            ani.SetTrigger("Heal");
            if(Fist)
                Fist.SetActive(true);
        }
        if (phase == 2 && isMove)
        {
            GoToDestination();
        }
    }

    public void LaserToPlayer()
    {
        int i = 0;
        int index = 0;
        LineRenderer select = lasers[0];
        while (i < lasers.Count) 
        {
            if (lasers[i].transform.position.y < Entity.Player.transform.position.y)
            {
                select = lasers[i];
                index = i;
                i++;
            }
            else
            {
                break;
            }
        }
        StartCoroutine(LaserShow(select, index));
    }

    public void SpawnEnergyBall_Pattern()
    {
        if (isStop) return;
        int value = UnityEngine.Random.Range(0, 3);
        switch (value) 
        {
            case 0:
                StartCoroutine(SpawnEnergyBall_Spiral(1, 20));
                break;
            case 1:
                SpawnEnergyBall(1, 10);
                break;
            case 2:
                SpawnEnergyBall(1, 20);
                break;
        }
    }

    public void SpawnEnergyBall(float radius, int SpawnCount)
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            float angle = i * MathF.PI * 2 / SpawnCount;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            float angleDegree = - angle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, angleDegree);
            GameObject obj = Make_New_EnergyBall(new Vector3(transform.position.x + x, transform.position.y + y, 0), rot);
            if (obj != null)
            {
                obj.GetComponent<HitColider>().owner = owner;
                obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y) * 5000);
            }
        }
    }

    public GameObject Make_New_EnergyBall(Vector3 pos, Quaternion rot)
    {
        GameObject obj = null;
        foreach (GameObject eb in EnergyBalls) 
        {
            if(eb)
                if(eb.activeSelf == false)
                {
                    eb.transform.position = pos;
                    eb.transform.rotation = rot;
                    eb.SetActive(true);
                    return eb;
                }
        }
        if (CurrentEnergyBallCount < MaxCurrentEnergyBallCount)
        {
            obj = Instantiate(EnergyBall, pos, rot);
            EnergyBalls[CurrentEnergyBallCount] = obj;
            CurrentEnergyBallCount++;
        }
        
        return obj;
    }

    // 애니메이션 이벤트 용
    public void NoStop() 
    {
        isStop = false;
        delayMoveTime = 0;
    }

    IEnumerator MovePos()
    {
        while (MoveTrs.Length > 0 && !owner.isDie)
        {
            yield return new WaitForSeconds(delayMoveTime);
            if (!isStop && !isMove)
            {
                delayMoveTime = delayMoveTime_origin;
                isMove = true;

                int randomValue = UnityEngine.Random.Range(-2, 2);
                if (Mathf.Abs(randomValue) == 1)
                {
                    isStop = true;
                    if (ani)
                    {
                        ani.SetTrigger("Laser");
                    }
                    destination = MoveTrs[UnityEngine.Random.Range(0, MoveTrs.Length)].position + new Vector3(20 * randomValue, 0);
                }
                else
                {
                    destination = MoveTrs[UnityEngine.Random.Range(0, MoveTrs.Length - 1)].position;
                }
            }
        }
    }

    public void SpawnMinion()
    {
        if (owner.GetHp() < owner.maxHP * 0.75)
        {
            if (!owner.isDie)
            {
                Instantiate(Minion, transform.position, Quaternion.identity).GetComponent<HitColider>().owner = owner;
            }
        }
    }


    IEnumerator LaserShow(LineRenderer laser, int index)
    {
        WarningBox[index].SetActive(true);
        
        yield return new WaitForSeconds(1f);

        WarningBox[index].SetActive(false);
        laser.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        laser.gameObject.SetActive(false);
    }

    IEnumerator SpawnEnergyBall_Spiral(float radius, int SpawnCount)
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            yield return new WaitForSeconds(0.01f);
            float angle = i * MathF.PI * 2 / SpawnCount;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            float angleDegree = -angle * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0, 0, angleDegree);
            GameObject obj = Make_New_EnergyBall(new Vector3(transform.position.x + x, transform.position.y + y, 0), rot);
            if (obj != null)
            {
                obj.GetComponent<HitColider>().owner = owner;
                obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y) * 5000);
            }
        }
    }
}
