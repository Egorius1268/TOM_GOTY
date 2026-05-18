// TurretData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Turret Data")]
public class TurretData : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public Sprite worldSprite;
    public Color towerColor = Color.white; 
    public int cost;
    public bool isTrap;
    

    [Header("Combat")]
    public float damage;
    public float range;
    public float fireRate; // bullets per second
    
    [Header("Bullet Type")]
    public BulletType bulletType; // SO с типом пули
    public GameObject bulletPrefab; // базовый префаб пули
    
    [Header("Visual")]
    public GameObject prefab; 
    
    
     
}