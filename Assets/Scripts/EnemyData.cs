
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float moveSpeed = 2f;
    public int maxHealth = 100;
    public int damageToPlayer = 5;
    public int moneyReward = 10;
    public GameObject prefab;
    public Sprite sprite;
}