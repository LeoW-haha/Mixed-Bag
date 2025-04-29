using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public GameObject enemyPrefab;
        public Sprite enemySprite;
        public float spawnWeight = 1f;
    }

    [Header("Spawn Settings")]
    [SerializeField] private EnemyType[] enemyTypes;
    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 10f;
    [SerializeField] private int maxEnemiesAtOnce = 3;
    
    [Header("Path Settings")]
    [SerializeField] private Transform[] pathWaypoints;
    
    private int activeEnemies = 0;

    private void Start()
    {
        if (enemyTypes == null || enemyTypes.Length == 0)
        {
            Debug.LogError("EnemySpawner: No enemy types configured!");
            enabled = false;
            return;
        }

        if (pathWaypoints == null || pathWaypoints.Length < 2)
        {
            Debug.LogError("EnemySpawner: Need at least 2 waypoints for a path!");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (activeEnemies < maxEnemiesAtOnce)
            {
                SpawnEnemy();
            }

            float spawnDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        // Calculate total weight
        float totalWeight = 0f;
        foreach (EnemyType enemyType in enemyTypes)
        {
            totalWeight += enemyType.spawnWeight;
        }

        // Select random enemy type based on weights
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        EnemyType selectedType = enemyTypes[0];

        foreach (EnemyType enemyType in enemyTypes)
        {
            currentWeight += enemyType.spawnWeight;
            if (randomValue <= currentWeight)
            {
                selectedType = enemyType;
                break;
            }
        }

        // Spawn the enemy
        GameObject enemyObj = Instantiate(selectedType.enemyPrefab);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        
        if (enemy != null)
        {
            // Set up the enemy
            SpriteRenderer spriteRenderer = enemyObj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && selectedType.enemySprite != null)
            {
                spriteRenderer.sprite = selectedType.enemySprite;
            }

            enemy.Initialize(pathWaypoints);
            activeEnemies++;

            // Subscribe to enemy destruction
            StartCoroutine(WatchEnemy(enemyObj));
        }
        else
        {
            Debug.LogError("EnemySpawner: Enemy prefab missing Enemy component!");
            Destroy(enemyObj);
        }
    }

    private IEnumerator WatchEnemy(GameObject enemy)
    {
        while (enemy != null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        activeEnemies--;
    }
} 