using UnityEngine;

public class EnemyAttackBarrier : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private LayerMask barrierLayer;

    private EnemyMovement movement;
    private BarrierWall currentBarrier;
    private float attackTimer;
    private bool isAttacking = false;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        if (isAttacking && currentBarrier != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackSpeed)
            {
                Attack();
                attackTimer = 0f;
            }
        }
       
        if (isAttacking && currentBarrier == null)
        {
            ResumeMovement();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((barrierLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            BarrierWall barrier = collision.gameObject.GetComponent<BarrierWall>();
            if (barrier != null)
            {
                currentBarrier = barrier;
                isAttacking = true;
                attackTimer = 0f;
                
                if (movement != null)
                {
                    movement.moveSpeed = 0f; 
                }
            }
        }
    }

    private void Attack()
    {
        if (currentBarrier != null)
        {
            currentBarrier.TakeDamage(attackDamage);
            // GetComponent<Animator>().SetTrigger("Attack");
        }
    }

    private void ResumeMovement()
    {
        isAttacking = false;
        currentBarrier = null;
        if (movement != null && movement.enemyData != null)
        {
            movement.moveSpeed = movement.enemyData.moveSpeed;
        }
    }
}
