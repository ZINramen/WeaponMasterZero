using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float install_ZValue;
    public enum BulletType
    {
        General = 0,
        Parring,
    }

    public BulletType bulletType;
    [SerializeField] private float deleteTime = 2f;

    public void SetDeleteTime(float inputDeleteTime)
    {
        deleteTime = inputDeleteTime;
    }

    private Vector3 shootingDir = new Vector3(0, 0, 0);
    
    private Rigidbody2D myRd;
    public float addForce = 5f;
    
    [SerializeField] private float parrigForce = 5f;

    [SerializeField] private bool isPlayerParring;
    public int wallParringHP = 2;
    private GameObject m_player;
    
    private AudioSource audioSource;

    public bool Get_IsPlayerParring()
    {
        return isPlayerParring;
    }
    
    private void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        shootingDir.z = install_ZValue;
        
        myRd = this.gameObject.GetComponent<Rigidbody2D>();
        
        ShootingBullet(addForce, Quaternion.Euler(shootingDir));
        Invoke("BrokenBullet", deleteTime);
    }

    public void BrokenBullet()
    {
        //Destroy(this.gameObject);
    }

    public void DestoryBullet()
    {
        if (m_player)
            m_player.gameObject.GetComponent<StopTime>().PlayALLTime();
        Destroy(this.gameObject);
    }

    public void Parring(GameObject player)
    {
        m_player = player.gameObject.transform.parent.gameObject;
        m_player.gameObject.GetComponent<StopTime>().timeScale = 0.2f;
        m_player.gameObject.GetComponent<StopTime>().DelayTime();
        StopMove();
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        this.gameObject.GetComponent<HitColider>().owner = m_player.GetComponent<Entity>();
        ShootingBullet(parrigForce, Quaternion.Euler(0, 0, angle));

        isPlayerParring = true;
        audioSource.Play();
        StartCoroutine("returnTimeDelay", 0.3f);
    }

    private IEnumerator returnTimeDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        m_player.gameObject.GetComponent<StopTime>().PlayALLTime();
    }

    public void StopMove()
    {
        myRd.velocity = Vector2.zero;
        myRd.angularVelocity = 0f;
    }

    public void ShootingBullet(float attackPower, Quaternion angle)
    {
        transform.rotation = angle;
        myRd.AddForce(this.transform.right * attackPower, ForceMode2D.Impulse);
    }
}
