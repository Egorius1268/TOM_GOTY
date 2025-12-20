using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refereneces")]
    [SerializeField] private EnemyData[] enemyDatas;


    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (!isSpawning) return;
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0) {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesAlive == 0 && enemiesLeftToSpawn == 0) {
            EndWave();
        }
    }

    private void EnemyDestroyed() {
        enemiesAlive--;
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
    }

    private void EndWave() {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;
        StartCoroutine(StartWave());
    }

    private void SpawnEnemy()
    {
        EnemyData enemyData = enemyDatas[0];
        GameObject prefabToSpawn = enemyData.prefab;
        if (prefabToSpawn == null)
        {
            Debug.LogError($"No prefab assigned in {enemyData.name}!");
            return;
        }
        GameObject enemy = Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
        //Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
        
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.enemyData = enemyData;
        }
        else
        {
            Debug.LogWarning("Enemy prefab doesn't have EnemyMovement component!");
        }
        
        Health health = enemy.GetComponent<Health>();
        if (health != null)
        {
            health.enemyData = enemyData;
        }
        else
        {
            Debug.LogWarning("Enemy prefab doesn't have Health component!");
        }
    }
    
    private EnemyData GetEnemyDataForWave()
    {
       
        if (currentWave <= 3 && enemyDatas.Length > 0)
            return enemyDatas[0]; 
        
        if (currentWave > 3 && currentWave <= 6 && enemyDatas.Length > 1)
            return enemyDatas[1]; 
        
        if (currentWave > 6 && enemyDatas.Length > 2)
            return enemyDatas[2]; 
        
        
        int randomIndex = Random.Range(0, enemyDatas.Length);
        return enemyDatas[randomIndex];
    }

    private int EnemiesPerWave() { 
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }

}
