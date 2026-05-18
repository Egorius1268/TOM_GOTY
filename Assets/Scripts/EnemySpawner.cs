using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refereneces")]
    [SerializeField] private EnemyData[] enemyDatas;
    [SerializeField] private LevelWavesData levelData; 
    

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private int maxWaves = 4;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    
    public int currentWave = 1;
    public TMP_Text currentWaveText;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;
    private bool levelCompleted = false;
    
    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
        UpdateWaveDisplay();
    }

    private void Start()
    {
        StartCoroutine(StartWaveCoroutine(levelData.timeBeforeFirstWave));
    }
    

    private void EnemyDestroyed() {
        enemiesAlive--;
        if (!isSpawning && enemiesAlive <= 0)
        {
            EndWave();
        }
    }
    private void OnDestroy()
    {
        onEnemyDestroy.RemoveListener(EnemyDestroyed);
    }
    private IEnumerator StartWaveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        int waveIndex = currentWave - 1;
        if (waveIndex < levelData.waves.Length)
        {
            WaveConfig currentWaveConfig = levelData.waves[waveIndex];
            StartCoroutine(SpawnWaveGroupsRoutine(currentWaveConfig)); // корутина для групп врагов
        }
    }

    private IEnumerator SpawnWaveGroupsRoutine(WaveConfig wave)
    {
        isSpawning = true;
        foreach (EnemyGroup group in wave.enemyGroups)
        {
            if (group.enemyType == null || group.enemyType.prefab == null) continue;
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyType);
                enemiesAlive++;
                yield return new WaitForSeconds(group.spawnInterval);
            }
        }
        isSpawning = false; 
        if (enemiesAlive <= 0)
        {
            EndWave();
        }
        
    }

    private void EndWave()
    {
        if (currentWave >= levelData.waves.Length)
        {
            if (!levelCompleted)
            {
                levelCompleted = true;
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.CompleteLevel();
                }
            }
            return;
        }

        currentWave++;
        UpdateWaveDisplay();
        
        StartCoroutine(StartWaveCoroutine(levelData.timeBetweenWaves));
    }
    
    

    private void SpawnEnemy(EnemyData enemyData)
    {
        GameObject prefabToSpawn = enemyData.prefab;
        if (prefabToSpawn == null)
        {
            return;
        }
        GameObject enemy = Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.enemyData = enemyData;
        }
        
        Health health = enemy.GetComponent<Health>();
        if (health != null) health.enemyData = enemyData;
        if (prefabToSpawn == null)
        {
            Debug.LogError($"No prefab assigned in {enemyData.name}!");
            return;
        }
        //Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
        
        
    }
    
    
    private void UpdateWaveDisplay()
    {
        currentWaveText.text = currentWave.ToString();
    }

    // private void timeSinceLastSpawn(float val) {} 

}
