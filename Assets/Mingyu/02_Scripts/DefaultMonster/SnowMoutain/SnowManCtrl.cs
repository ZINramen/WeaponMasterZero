using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowManCtrl : Default_Monster
{
    [SerializeField] private GameObject IceBall;
    private Transform shootingPos;
    private GameObject dummyIceBall;
    
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
        dummyIceBall = GameObject.Instantiate(IceBall, shootingPos.position, Quaternion.identity);

        // Vector2 startPos = this.transform.position;
        // Vector2 endPos = this.gameObject.GetComponent<Default_Monster>().player_pos;
        // Vector2 v2 = endPos - startPos;
        //
        // float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        dummyIceBall.GetComponent<IceShooting_Ctrl>().install_ZValue 
            = this.transform.position.x > player_pos.x ? 180 : 0;

        dummyIceBall.GetComponent<HitColider>().owner = this.gameObject.GetComponent<Entity>();
    }

    public void Delay_EndAttack()
    {
        isShooting = true;
    }
}
