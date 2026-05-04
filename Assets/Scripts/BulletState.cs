using UnityEngine;

public enum BulletState
{
    Normal,      // обычная пуля
    Fire,        // огненная - наносит доп урон со временем
    Ice,         // ледяная - замедляет врагов
    Electric,    // электрическая - наносит урон нескольким врагам
    Poison,      // ядовитая - наносит урон со временем
    ArmorPiercing // бронебойная - игнорирует часть брони (спорно)
}


[CreateAssetMenu(menuName = "Tower Defense/Bullet Type")]
public class BulletType : ScriptableObject
{
    public BulletState state;
    public Color bulletColor = Color.white;
    public Sprite bulletSprite;
    public float damageMultiplier = 1f;
    
    [Header("Fire Effect")]
    public bool hasBurnEffect = false;
    public float burnDamagePerSecond = 2f;
    public float burnDuration = 3f;
    
    [Header("Ice Effect")]
    public bool hasSlowEffect = false;
    public float slowPercentage = 0.5f; // 50% замедление
    public float slowDuration = 2f;
    
    [Header("Electric Effect")]
    public bool hasChainEffect = false;
    public int maxChainTargets = 3;
    public float chainDamageReduction = 0.7f; // 70% урона на следующую цель
    
    [Header("Visual Effects")]
    public ParticleSystem hitEffect;
    public GameObject trailEffect;
}