using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Level Waves Data")]
public class LevelWavesData : ScriptableObject
{
    [Header("Level config")]
    public float timeBeforeFirstWave = 5f; 
    public float timeBetweenWaves = 5f;    
    
    [Header("Waves")]
    public WaveConfig[] waves;            
}