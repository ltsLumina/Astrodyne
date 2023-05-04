using System;
using System.Collections;
using UnityEngine;

// TEMPORARY SCRIPT FOR FUNNIEZ
public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnDelay = 1.5f;

    void Update() => StartCoroutine(SpawnEnemy());

    IEnumerator SpawnEnemy()
    {
        var randomSpawnPos = new Vector2(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-4f, 4f));

        while (true)
        {
            Instantiate(enemyPrefab, randomSpawnPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}