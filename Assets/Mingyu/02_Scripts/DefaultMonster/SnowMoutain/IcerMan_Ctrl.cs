using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

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
                if (other.gameObject.GetComponent<HitColider>().attType == HitColider.AttackType.Player_GunAtt
                    && other.gameObject.GetComponent<HitColider>().owner != icerEntity)
                {
                    Debug.Log("맞음" + other.gameObject.GetComponent<HitColider>().attType);
                    
                     Vector3 StartPoint = new Vector3(this.transform.position.x + 0.1f, this.transform.position.y - 0.1f,
                         this.transform.position.z);
                     
                     //GameObject dummyBullet = GameObject.Instantiate(Parringbullet, this.transform.position, Quaternion.identity);
                     //dummyBullet.gameObject.GetComponent<HitColider>().owner = icerEntity;
                     
                     //float randomX_Angle = Random.Range(-0.05f, 0.05f);
                     //float randomY_Angle = Random.Range(-0.05f, 0.05f);
                     
                     ///Vector3 EndPoint = new Vector3(other.transform.position.x + randomX_Angle, other.transform.position.y + randomY_Angle, other.transform.position.z);
                     
                     // Vector3 AttackDir = ( StartPoint - EndPoint).normalized;
                     
                    //dummyBullet.gameObject.GetComponent<Rigidbody2D>().AddForce(parringSpeed * -AttackDir, ForceMode2D.Impulse);
                }
            }
        }
}
