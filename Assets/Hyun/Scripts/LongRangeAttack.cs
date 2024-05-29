using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // 발사체 Prefab
    public float projectileSpeed = 10f;
    
    GameObject player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");    
    }
    public void FireProjectile()
    {
        Vector2 direction = Vector2.one;
        Vector2 maxHeightDisplacement = new Vector2((player.transform.position.x - transform.position.x)/1.5f, Mathf.Abs(player.transform.position.x - transform.position.x)/4);;
        // 발사체 인스턴스 생성
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<HitColider>().owner= GetComponent<Entity>();
        
        // 발사체를 원하는 방향으로 이동시키는 물리적 힘을 추가
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * projectileSpeed;

        // m*k*g*h = m*v^2/2 (단, k == gravityScale) <= 역학적 에너지 보존 법칙 적용
        float v_y = Mathf.Sqrt(2 * rb.gravityScale * -Physics2D.gravity.y * maxHeightDisplacement.y);
        // 포물선 운동 법칙 적용
        float v_x = maxHeightDisplacement.x * v_y / (2 * maxHeightDisplacement.y);

        Vector2 force = rb.mass * (new Vector2(v_x, v_y) - rb.velocity);
        rb.AddForce(force, ForceMode2D.Impulse);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
