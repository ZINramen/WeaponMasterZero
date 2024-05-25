using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLongRangeAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // 투사체 프리팹
    public float projectileSpeed = 10f; // 투사체 속도
    public Transform attackPosition; // 투사체가 발사될 위치
    public Entity owner; // 투사체의 owner

    // Update is called once per frame
    void Update()
    {
        // 투사체 발사는 AISystem에서 처리합니다.
    }

    public void FireProjectile() // 이 메서드를 public으로 변경했습니다.
    {
        // 투사체 프리팹 인스턴스화
        GameObject projectile = Instantiate(projectilePrefab, attackPosition.position, Quaternion.identity);

        // 투사체를 앞으로 발사
        projectile.GetComponent<Rigidbody2D>().velocity = transform.right * projectileSpeed;

        // 투사체의 owner 설정
        HitColider hitColider = projectile.GetComponent<HitColider>();
        if (hitColider != null)
        {
            hitColider.owner = owner;
        }
    }
    
}