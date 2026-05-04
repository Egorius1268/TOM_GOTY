using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet5 : MonoBehaviour
{

    [Header("References")] [SerializeField]
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    Coroutine lifeCoroutine;
    [Header("Attributes")] 
    [SerializeField] private float bulletSpeed = 5f;
    private ParticleSystem particleEffect;
    private GameObject activeTrail;
    private float baseDamage;
    private Transform target;
    private Vector2 fixedDirection;
    
    
    
    public float lifeTime;
    public BulletState currentState = BulletState.Normal;
    public BulletType bulletType; 
    public ObjectPool pool;
    
    private Vector2 initialDirection;
   
    
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
        rb.freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();        
    }

    void OnEnable()
    {
        // сброс физики
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        
        // запуск таймер жизни
        lifeCoroutine = StartCoroutine(ReturnAfter(lifeTime));
        
        ApplyBulletVisuals();
    }
    public void SetDirection(Vector2 direction)
    {
        initialDirection = direction;
        
        
        rb.linearVelocity = initialDirection * bulletSpeed;
    }

    void OnDisable()
    {
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }
        if (activeTrail != null)
        {
            Destroy(activeTrail);
            activeTrail = null;
        }
        
    }

    void ApplyBulletVisuals()
    {
        if (bulletType != null)
        {
            currentState = bulletType.state;
            
            //  цвет пули
            if (spriteRenderer != null)
                spriteRenderer.color = bulletType.bulletColor;
            
            // спрайт
            if (bulletType.bulletSprite != null && spriteRenderer != null)
                spriteRenderer.sprite = bulletType.bulletSprite;
            
            // эффект хвоста
            if (bulletType.trailEffect != null)
            {
                activeTrail = Instantiate(bulletType.trailEffect, transform);
                activeTrail.transform.localPosition = Vector3.zero;
            }
            
            if (trailRenderer != null)
            {
                trailRenderer.startColor = bulletType.bulletColor;
                trailRenderer.endColor = new Color(bulletType.bulletColor.r, 
                    bulletType.bulletColor.g, bulletType.bulletColor.b, 0);
            }
        }
    }
    IEnumerator ReturnAfter(float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool();
    }

    public void SetDamage(float turretDamage)
    {
        baseDamage = turretDamage;
        if (bulletType != null)
        {
            baseDamage *= bulletType.damageMultiplier;
        }
    }

    /*public void SetTarget(Transform _target)
    {
        target = _target;
    }*/

    public void SetPool(ObjectPool bulletPool)
    {
        pool = bulletPool;
    }
    
    public void SetBulletType(BulletType type)
    {
        bulletType = type;
        if (bulletType != null)
        {
            currentState = bulletType.state;
            ApplyBulletVisuals();
        }
    }

    // Update is called once per frame
    /*private void FixedUpdate()
    {
        if (!target)
        {
            if (gameObject.activeSelf)
                ReturnToPool();
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;

        rb.linearVelocity = direction * bulletSpeed;
    }*/

    private void OnCollisionEnter2D(Collision2D other)
    {

        Health health = other.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage((int)baseDamage);
            ApplySpecialEffects(other.gameObject);
        }
        SpawnHitEffect(other.contacts[0].point);
        ReturnToPool();

        //Destroy(gameObject);
    }

    private void ApplySpecialEffects(GameObject targetObject)
    {
        if (bulletType == null) return;
        
        Health targetHealth = targetObject.GetComponent<Health>();
        if (targetHealth == null) return;
        
        
        switch (currentState)
        {
            case BulletState.Fire:
                if (bulletType.hasBurnEffect)
                {
                    ApplyBurnEffect(targetObject);
                }
                break;
                
            case BulletState.Ice:
                if (bulletType.hasSlowEffect)
                {
                    ApplySlowEffect(targetObject);
                }
                break;
                
            case BulletState.Electric:
                if (bulletType.hasChainEffect)
                {
                    ApplyChainEffect(targetObject);
                }
                break;
                
            case BulletState.Poison:
                ApplyPoisonEffect(targetObject);
                break;
        }
    }
    
    private void ApplyBurnEffect(GameObject target)
    {
        EnemyMovement enemyMovement = target.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.ApplyBurnEffect(bulletType.burnDamagePerSecond, bulletType.burnDuration);
        }
    }
    
    private void ApplySlowEffect(GameObject target)
    {
        EnemyMovement enemyMovement = target.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.ApplySlowEffect(bulletType.slowPercentage, bulletType.slowDuration);
        }
    }
    
    private void ApplyChainEffect(GameObject initialTarget)
    {
        
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            initialTarget.transform.position, 
            3f, 
            LayerMask.GetMask("Enemy")
        );
        
        int chainCount = 0;
        float currentDamage = baseDamage;
        
        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            if (enemyCollider.gameObject == initialTarget) continue;
            
            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                // Уменьшаем урон с каждой цепью
                currentDamage *= bulletType.chainDamageReduction;
                enemyHealth.TakeDamage((int)currentDamage);
                
                chainCount++;
                if (chainCount >= bulletType.maxChainTargets) break;
            }
        }
    }
    
    private void ApplyPoisonEffect(GameObject target)
    {
        EnemyMovement enemyMovement = target.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            
            enemyMovement.ApplyPoisonEffect(baseDamage * 0.3f, 5f); // 30% урона ядом в течение 5 секунд
        }
    }
    
    private void SpawnHitEffect(Vector2 position)
    {
        if (bulletType != null && bulletType.hitEffect != null)
        {
            ParticleSystem effect = Instantiate(bulletType.hitEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }
    }
    
    void ReturnToPool()
    {
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (pool != null)
        {
            pool.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
            
    
