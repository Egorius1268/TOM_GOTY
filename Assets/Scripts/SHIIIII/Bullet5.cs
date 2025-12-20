using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet5 : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    Coroutine lifeCoroutine;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;

    public float lifeTime;
    public ObjectPool pool;
   // public int bulletDamage;
   private float damage; 

    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    

    public void SetDamage(float turretDamage)
    {
        damage = turretDamage;
    }
    
    public void SetTarget(Transform _target) {  
        target = _target;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!target) return;
        Vector2 direction = (target.position - transform.position).normalized;

        rb.linearVelocity = direction * bulletSpeed;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        Health health = other.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage((int)damage);
        }
        else
        {
            Debug.Log($"Bullet hit {other.gameObject.name}, but it has no Health component!");
        }

        Destroy(gameObject);
    }
}
