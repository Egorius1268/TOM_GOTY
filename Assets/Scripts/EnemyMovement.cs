using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameManager gameManager;
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Attributes")]
    private float moveSpeed;
    [SerializeField] private float waypointThreshold = 0.1f; 
    
    [Header("Status Effects")]
    private bool isBurning = false;
    private bool isSlowed = false;
    private bool isPoisoned = false;
    private float originalMoveSpeed;
    private Coroutine burnCoroutine;
    private Coroutine slowCoroutine;
    private Coroutine poisonCoroutine;
    
    private Transform target;
    private int pathIndex = 0;
    
    public EnemyData enemyData;
    

    private void Start()
    {
        
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null) 
            Debug.LogError("No game manager in scene");
        
        if (enemyData != null)
        {
            if (spriteRenderer != null && enemyData.sprite != null)
                spriteRenderer.sprite = enemyData.sprite;
            moveSpeed = enemyData.moveSpeed;
        }
        if (LevelManager.main != null && LevelManager.main.path.Length > 0)
        {
            target = LevelManager.main.path[pathIndex];
        }
        else
        {
            Debug.LogError("level manager or path not found");
        }
        originalMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position);
            if (direction.x > 0.01f)
            {
                spriteRenderer.flipX = false; 
            }
            else if (direction.x < -0.01f)
            {
                spriteRenderer.flipX = true; 
            }
        }
    }
    private void FixedUpdate()
    {
        if (target == null) return;
        
        Vector2 direction = (target.position - transform.position);
        float distanceToTarget = direction.magnitude;
        
        if (distanceToTarget <= waypointThreshold)
        {
            UpdateToNextWaypoint();
        }
        else
        {
            rb.linearVelocity = direction.normalized * moveSpeed;
        }
    }

    private void UpdateToNextWaypoint()
    {
        rb.linearVelocity = Vector2.zero;
        
        transform.position = target.position;
        pathIndex++;
        
        if (pathIndex >= LevelManager.main.path.Length)
        {
            ReachedEndOfPath();
            return;
        }
        
        target = LevelManager.main.path[pathIndex];
    }

    private void ReachedEndOfPath()
    {
        int damage = enemyData?.damageToPlayer ?? 5;
        gameManager.DamageDealed(damage);
        
        EnemySpawner.onEnemyDestroy.Invoke();
        Destroy(gameObject);
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
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = Color.red;
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(Mathf.RoundToInt(dps * 0.5f)); 
        }
        
        float damageInterval = 1f; 
        float nextDamageTime = damageInterval;
    
        while (elapsed < duration)
        {
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
        
        sr.color = originalColor;
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
        moveSpeed = originalMoveSpeed * (1f - slowPercent); 
        
       
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = Color.cyan;
        
        yield return new WaitForSeconds(duration);
        
        sr.color = originalColor;
        moveSpeed = originalMoveSpeed;
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
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = Color.green;
        
        for (int i = 0; i < ticks; i++)
        {
            Health health = GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage((int)damagePerTick);
            }
            
            sr.color = i % 2 == 0 ? Color.green : new Color(0, 0.7f, 0, 1);
            
            yield return new WaitForSeconds(0.5f);
        }
        sr.color = originalColor;
        isPoisoned = false;
        poisonCoroutine = null;
    }
    
    
    
}