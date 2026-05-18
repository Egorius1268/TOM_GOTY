using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Attributes")]
    public float moveSpeed;
    [SerializeField] private float waypointThreshold = 0.1f; 
    
    private Transform target;
    private int pathIndex = 0;
    public EnemyData enemyData;
    

    private void Start()
    {
        
        if (enemyData != null)
        {
            if (spriteRenderer != null && enemyData.worldSprite != null)
                spriteRenderer.sprite = enemyData.worldSprite;
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
        
        if (moveSpeed <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DamageDealed(damage);
        }
        
        EnemySpawner.onEnemyDestroy.Invoke();
        Destroy(gameObject);
    }
    
}