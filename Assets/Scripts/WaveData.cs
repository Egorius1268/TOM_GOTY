using UnityEngine;

[System.Serializable]
public class EnemyGroup
{
    public EnemyData enemyType; // маленькие подгруппы из одинаковых врагов
    public int count;
    public float spawnInterval;
}

[System.Serializable]
public class WaveConfig
{
    public string waveName;
    [Header("Enemies in wave")]
    public EnemyGroup[] enemyGroups; 
}