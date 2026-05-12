using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    private GameManager gameManager;
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    private EnemyMovement movement;
    private Health health;
    private Color defaultColor;
    
    [Header("Attributes")]
    private float moveSpeed;
     
    
    [Header("Status Effects")]
    private bool isBurning = false;
    private bool isSlowed = false;
    private bool isPoisoned = false;
    
    private Coroutine burnCoroutine;
    private Coroutine slowCoroutine;
    private Coroutine poisonCoroutine;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<Health>();
        sr = GetComponent<SpriteRenderer>();
        defaultColor = sr.color; 
    }
    
    public void ApplyBurnEffect(float damagePerSecond, float duration)
    {
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);
            
        burnCoroutine = StartCoroutine(BurnEffect(damagePerSecond, duration));
    }
    
    private IEnumerator BurnEffect(float dps, float duration)
    {
        isBurning = true;
        float elapsed = 0f;
        
        // Визуальный эффект горения
        
        Color originalColor = sr.color;
        sr.color = Color.red;
        if (health != null)
        {
            health.TakeDamage(Mathf.RoundToInt(dps * 0.5f)); 
        }
        
        float damageInterval = 1f; 
        float nextDamageTime = damageInterval;
    
        while (elapsed < duration)
        {
            if (health == null) yield break;
            elapsed += Time.deltaTime;
            
            if (elapsed >= nextDamageTime)
            {
                if (health != null)
                {
                    int damageThisTick = Mathf.RoundToInt(dps);
                    health.TakeDamage(damageThisTick);
                    Debug.Log($"Burn tick: {damageThisTick} HP at {elapsed}s");
                }
                nextDamageTime += damageInterval;
            }
            
            float alpha = Mathf.PingPong(Time.time * 10f, 0.3f) + 0.7f;
            sr.color = new Color(1f, alpha * 0.3f, alpha * 0.3f, 1f);
            
            yield return null;
            
        }
        if (sr != null) sr.color = defaultColor;
        isBurning = false;
        burnCoroutine = null;
    }
    
    public void ApplySlowEffect(float slowPercentage, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
            
        slowCoroutine = StartCoroutine(SlowEffect(slowPercentage, duration));
    }
    
    private IEnumerator SlowEffect(float slowPercent, float duration)
    {
        isSlowed = true;
        if (movement == null || movement.enemyData == null) yield break;
        float normalSpeed = movement.enemyData.moveSpeed;
        movement.moveSpeed = normalSpeed * (1f - slowPercent); 
        
        
        Color originalColor = sr.color;
        sr.color = Color.cyan;
        
        yield return new WaitForSeconds(duration);
        
        if (this != null) 
        {
            if (sr != null) sr.color = defaultColor;
            if (movement != null) movement.moveSpeed = normalSpeed;
        }
        
        isSlowed = false;
        slowCoroutine = null;
    }
    
    public void ApplyPoisonEffect(float totalDamage, float duration)
    {
        if (poisonCoroutine != null)
            StopCoroutine(poisonCoroutine);
            
        poisonCoroutine = StartCoroutine(PoisonEffect(totalDamage, duration));
    }
    
    private IEnumerator PoisonEffect(float totalDamage, float duration)
    {
        isPoisoned = true;
        float damagePerTick = totalDamage / (duration / 0.5f); 
        int ticks = Mathf.FloorToInt(duration / 0.5f);
        
        Color originalColor = sr.color;
        sr.color = Color.green;
        
        for (int i = 0; i < ticks; i++)
        {
            if (health == null) yield break;
            if (health != null)
            {
                health.TakeDamage((int)damagePerTick);
            }
            
            sr.color = i % 2 == 0 ? Color.green : new Color(0, 0.7f, 0, 1);
            
            yield return new WaitForSeconds(0.5f);
        }
        if (sr != null) sr.color = defaultColor;
        isPoisoned = false;
        poisonCoroutine = null;
    }
    
    
    
}