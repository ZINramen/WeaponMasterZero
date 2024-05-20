using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DynamiteCtrl : MonoBehaviour
{
    private Rigidbody2D myRd;
    private CircleCollider2D myColl;
    public GameObject GunBoss;
    
    [SerializeField] private float upForce = 2;
    [SerializeField] private float forwardForce = 3;
    
    [SerializeField] private float minBoomTime = 0.2f;
    [SerializeField] private float maxBoomTime = 0.6f;

    [SerializeField] private float motionDelayTime = 0.05f;

    [SerializeField] private GameObject BoomObj;
    private GameObject dummyBoomObj;
    public enum playerDirection
    {
        right = 1,
        left = -1
    }

    public playerDirection pDir = playerDirection.right;
    private float randomBoomTime;
    
    // Start is called before the first frame update
    void Start()
    {
        if (BoomObj != this.gameObject)
        {
            myRd = this.gameObject.GetComponent<Rigidbody2D>();
            myRd.AddForce(this.transform.up * upForce, ForceMode2D.Impulse);
            myRd.AddForce((this.transform.right * (int)pDir) * forwardForce, ForceMode2D.Impulse);

            randomBoomTime = Random.Range(minBoomTime, maxBoomTime);
            Invoke("BossAttackDynamite", randomBoomTime - motionDelayTime);
        }
        else
        {
            GameObject boss = GameObject.FindWithTag("Boss").GetComponent<Boss>().gameObject;
            if (boss.GetComponent<Boss>().bossType == BossType.Gun)
            {
                GunBoss = boss;
                GunBoss.GetComponent<Boss>().TrustValue_Setting(this.gameObject);
            }
        }
    }

    private void BossAttackDynamite()
    {
        GunBoss.GetComponent<GunBoss>().AttackDynamite();
        Invoke("Boom", motionDelayTime);
    }
    
    private void Boom()
    {
        dummyBoomObj = Instantiate(BoomObj, this.transform.position, quaternion.identity);
        dummyBoomObj.GetComponent<HitColider>().owner = GunBoss.GetComponent<Entity>();
        Destroy(this.gameObject);
    }
    // 카메라 액션 추가 : 폭발에 반응
    public void BoomActionCam()
    {
        Animator camA = Camera.main.GetComponent<Animator>();
        if (camA != null)
        {
            camA.SetTrigger("Boom");
        }
    }

    public void DeletePref()
    {
        GunBoss.GetComponent<GunBoss>().EndP1Skill2_Attack();
        Destroy(this.gameObject);
    }
}
