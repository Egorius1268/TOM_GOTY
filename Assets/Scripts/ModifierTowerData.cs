using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Modifier Tower Data")]
public class ModifierTowerData : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public Sprite worldSprite;
    public Sprite rangeCircleSprite; 
    public int cost;
    
    [Header("Modifier Settings")]
    public BulletState modifierState; // Какое состояние дает
    public BulletType bulletType; 
    
    [Header("Visual")]
    public GameObject prefab; 
    public Color towerColor = Color.white;
    
    [Header("Modifier Range")]
    public float triggerRadius = 1.5f; // радиус триггера
    
    [Header("Special Effects")]
    public bool hasPermanentEffect = false; // постоянно ли действует
    public float effectDuration = 5f; // если эффект временный
    public int maxModificationsPerSecond = 10; // лимит модификаций в секунду (спорно)
}
