using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Mingyu : Default_Monster
{    
    [SerializeField] private float m_dAtt_dist;
    [SerializeField] private float m_traceDist;
    
    [SerializeField] private GameObject Parring_Exc;
    private GameObject dummy_ParringExc;
    
    [SerializeField] private Material hitMat;
    private Material originMat;

    private GameObject parringHit;
    
    void Start()
    {
        base.Start();
        monsterState = new Default_MonsterState();
        Init_StateValueData(ref monsterState);

        stopDelayTime = 1.5f;

        AttHitCol = this.gameObject.transform.GetChild(1).gameObject;
        AttHitCol.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        originMat = this.gameObject.GetComponent<SpriteRenderer>().material;
        
        parringHit = this.gameObject.transform.GetChild(0).gameObject;
        parringHit.gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    protected override void Init_StateValueData(ref Default_MonsterState state)
    {
        state.defaultAtt_dist = m_dAtt_dist;
        state.traceDistance = m_traceDist;
    }
    
    public void ParringCheck(int int_Parring)
    {
        bool isParringOn = int_Parring == 1 ? true : false;
        
        parringHit.gameObject.GetComponent<BoxCollider2D>().enabled = isParringOn;
    }

    public void ParringHit()
    {
        if (dummy_ParringExc == null)
        {
            Vector3 spawnPos = new Vector3(this.transform.position.x, 
                this.transform.position.y + 0.8f,
                this.transform.position.z);

            dummy_ParringExc = GameObject.Instantiate(Parring_Exc, spawnPos, Quaternion.identity);
            dummy_ParringExc.transform.parent = this.gameObject.transform;
        }
        else
        {
            dummy_ParringExc.gameObject.GetComponent<ParticleSystem>().Play();
        }
        
        this.gameObject.GetComponent<SpriteRenderer>().material = hitMat;
        Invoke("ReturnMat", 0.1f);
    }

    private void ReturnMat()
    {
        this.gameObject.GetComponent<SpriteRenderer>().material = originMat;
    }
}
