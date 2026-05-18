// TurretData.cs
using UnityEngine;

public enum BuildingType { Turret, Trap, Barrier }

[CreateAssetMenu(menuName = "Tower Defense/Turret Data")]
public class TurretData : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public Sprite worldSprite;
    public Sprite rangeCircleSprite; 
    public Color towerColor = Color.white; 
    public int cost;
    public bool isTrap;
    public BuildingType buildingType; 

    [Header("Combat")]
    public float damage;
    public float range;
    public float fireRate; // bullets per second
    
    [Header("Bullet Type")]
    public BulletType bulletType; // SO с типом пули
    public GameObject bulletPrefab; // базовый префаб пули
    public BulletState trapEffectState; 
    public float effectDuration = 3f; 
    
    [Header("Visual")]
    public GameObject prefab; 
    
    public int barrierHealth = 100; 
    
    
     
}