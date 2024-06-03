using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BulletSpawn(1.5f));
    }

    public IEnumerator BulletSpawn(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(bullet, transform.position, Quaternion.identity);
        }
    } 
}
