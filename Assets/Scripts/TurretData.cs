// TurretData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Turret Data")]
public class TurretData : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public int cost;

    [Header("Combat")]
    public float damage;
    public float range;
    public float fireRate; // bullets per second
    
    [Header("Bullet Type")]
    public BulletType bulletType; // ScriptableObject с типом пули
    public GameObject bulletPrefab; // Базовый префаб пули
    
    [Header("Visual")]
    public GameObject prefab; 
     
}