using System;
using UnityEngine;
using Enemies;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject target;
    [SerializeField] private float minimumSpawnTime;
    [SerializeField] private float maximumSpawnTime;
    [SerializeField] private int spawnRange;

    private float _timeUntilSpawn;

    void Awake()
    {
        SetTimeUntilSpawn();
    }

    void Update()
    {
        if (GameManager.Instance.GameState == GameStates.Playing)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        _timeUntilSpawn -= Time.deltaTime;
        if (_timeUntilSpawn <= 0)
        {
            // Instanciate Enemy with random spawn Position
            Vector3 spawnPosition = transform.position;
            spawnPosition.x = Random.Range(spawnPosition.x - spawnRange, spawnPosition.x + spawnRange);
            var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            // Set Enemy Target
            var enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.Target = target.transform;
                enemy.transform.LookAt(target.transform);
            }

            // Set random time until next spawn
            SetTimeUntilSpawn();
        }
    }

    private void SetTimeUntilSpawn()
    {
        _timeUntilSpawn = Random.Range(minimumSpawnTime, maximumSpawnTime);
    }
}