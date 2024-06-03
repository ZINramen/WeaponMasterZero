using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcerMan_Ctrl : Default_Monster
{
        [SerializeField] private float m_dAtt_dist;
        [SerializeField] private float m_traceDist;
        [SerializeField] private GameObject Parringbullet;
        [SerializeField] private float parringSpeed;

        private Entity icerEntity;
        private Transform ParringPos;
        
        void Start()
        {
            base.Start();
            monsterState = new Default_MonsterState();
            Init_StateValueData(ref monsterState);
    
            stopDelayTime = 1.5f;
    
            AttHitCol = this.gameObject.transform.GetChild(0).gameObject;
            AttHitCol.gameObject.GetComponent<BoxCollider2D>().enabled = false;

            ParringPos = this.gameObject.transform.GetChild(0).transform;

            // 야매
            icerEntity = this.gameObject.GetComponent<Entity>();
            icerEntity.playerFinalBoss = player.gameObject.GetComponent<Entity>();
            icerEntity.activeDesireWeapon = true;
            
            icerEntity.desireWeaponFinalBoss = (int)(HitColider.AttackType.Player_SwordAtt);
        }
    
        protected override void Init_StateValueData(ref Default_MonsterState state)
        {
            state.defaultAtt_dist = m_dAtt_dist;
            state.traceDistance = m_traceDist;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<HitColider>())
            {
                if (other.gameObject.GetComponent<HitColider>().attType == HitColider.AttackType.Player_GunAtt)
                {
                    Debug.Log("맞음" + other.gameObject.name);
                    
                     GameObject dummyBullet = GameObject.Instantiate(Parringbullet, ParringPos.position, Quaternion.identity);
                     dummyBullet.gameObject.GetComponent<HitColider>().owner = icerEntity;

                     Vector3 AttackDir = (this.transform.position - other.transform.position).normalized;
                     
                    dummyBullet.gameObject.GetComponent<Rigidbody2D>()
                        .AddForce(parringSpeed * -AttackDir, ForceMode2D.Impulse);

                }
            }
        }
}
