using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShooting_Ctrl : MonoBehaviour
{
    public float install_ZValue;
    [SerializeField] private float deleteTime = 2f;

    private Vector3 shootingDir = new Vector3(0, 0, 0);
    
    private Rigidbody2D myRd;
    public float addForce = 5f;

    private GameObject m_player;
    
    private void Start()
    {
        shootingDir.z = install_ZValue;
        
        myRd = this.gameObject.GetComponent<Rigidbody2D>();
        
        ShootingBullet(addForce, Quaternion.Euler(shootingDir));
        Invoke("BrokenIce", deleteTime);
    }

    public void BrokenIce()
    {
        Destroy(this.gameObject);
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
