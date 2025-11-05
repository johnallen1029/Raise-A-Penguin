using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // leopardseal prefab here
    public Transform spawnPoint; // The position at the end of the platform

    public Transform startPoint;

    public Transform endPoint; 
    public float spawnInterval = 10f; // Time between spawns

    public int poolSize = 5;

    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 15f; 
    private float timer;

    private float nextSpawnTime;
    private List<GameObject> enemyPool;

    void Start()
    {
        enemyPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemy.tag = "Enemy"; 
            enemyPool.Add(enemy);
        }

        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval); 
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnEnemy();
            timer = 0f;

            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnEnemy()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                // Calculate a random position along the line
                Vector3 randomPosition = Vector3.Lerp(startPoint.position, endPoint.position, Random.Range(0f, 1f));
                
                enemy.transform.position = randomPosition;
                enemy.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                enemy.SetActive(true);
                return; 
            }
        }
    }
}