using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public class MobSpawner : MonoBehaviour
{
    public UnityEvent enemyCleared;

    [SerializeField] private Transform[] MobSpawnPoint;
    [SerializeField] private GameObject[] MobPrefab;

    private int enemyCount = 0; // Add this line

    public Transform Monsters;

    public void CheckMobStart()
    {
        StartCoroutine(CheckMob());
    }

    public void SpawnMob()
    {
        foreach (var pos in MobSpawnPoint)
        {
            Instantiate(MobPrefab[Random.Range(0, MobPrefab.Length)], pos.position, Quaternion.identity);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //if (other.CompareTag("Enemy"))
        //{
            //enemyCount++; // Increment the enemy count
        //}
    }

    IEnumerator CheckMob()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (Monsters.childCount == 0) // Check if the enemy count is zero
            {
                enemyCleared?.Invoke();
                StopAllCoroutines();
            }
            enemyCount = 0; // Reset the enemy count
        }
    }
}


