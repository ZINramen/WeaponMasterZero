using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class GunMon_Mingyu : Default_Monster
{
    [SerializeField] private GameObject ParringBullet;
    private Transform shootingPos;
    private GameObject dummyParringBullet;
    public GameObject ShootEffect;

    [SerializeField] private float m_dAtt_dist;
    [SerializeField] private float m_traceDist;
    
    private bool isShooting = false;
    private float attDelayCount = 0f;
    
    void Start()
    {
        base.Start();
        monsterState = new Default_MonsterState();
        Init_StateValueData(ref monsterState);

        shootingPos = this.gameObject.transform.GetChild(0).gameObject.transform;
    }

    protected override void Init_StateValueData(ref Default_MonsterState state)
    {
        state.defaultAtt_dist = m_dAtt_dist;
        state.traceDistance = m_traceDist;
    }

    protected override void UpdateSetting()
    {
        if (isShooting)
        {
            attDelayCount += Time.deltaTime;

            if (attDelayCount >= 0.5f)
            {
                isShooting = false;
                attDelayCount = 0f;
                
                EndAttack();
            }
        }
    }


    public void ShootingAttack()
    {
        this.transform.rotation = Quaternion.Euler(0, this.transform.position.x > player_pos.x ? 0 : 180, 0);
        dummyParringBullet = GameObject.Instantiate(ParringBullet, shootingPos.position, Quaternion.identity);
        GameObject.Instantiate(ShootEffect, shootingPos.position, Quaternion.identity);

        Vector2 startPos = this.transform.position;
        Vector2 endPos = this.gameObject.GetComponent<Default_Monster>().player_pos;
        Vector2 v2 = endPos - startPos;
        
        float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        dummyParringBullet.GetComponent<BulletCtrl>().install_ZValue = angle;

        dummyParringBullet.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
    }

    public void DieSetting()
    {
        this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
        this.gameObject.transform.GetChild(2).gameObject.GetComponent<Animator>().SetBool("isActive", true);
        
        Debug.Log("죽음");
    }

    public void Delay_EndAttack()
    {
        isShooting = true;
    }
}
