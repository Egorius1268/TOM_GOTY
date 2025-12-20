using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret1 : MonoBehaviour
{
    public TurretData data;
    [Header("References")]
   // [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
   //[SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private ObjectPool bulletPool;

    [Header("Attribute")] 
    //[SerializeField] private float targetingRange = 5f;
   // [SerializeField] private float rotationSpeed = 5f;
   // [SerializeField] private float bps = 1f;
    private float targetingRange;
    private float bps;
    private float turretDamage;
    private Transform target;
    private float timeUntilFire;
    

    private void Awake()
    {
        if (data != null)
        {
            InitializeFromData();
        }
    }
    public void InitializeFromData()
    {
        
        targetingRange = data.range;
        bps = data.fireRate;
        gameObject.name = data.name + " (Turret)";
        turretDamage = data.damage;
        if (bulletPool == null)
        {
            FindOrCreateBulletPool();
        }
    }
    
    void Start()
    {
        Debug.Log($"Built {data.name}, cost: {data.cost}");
    }
    private void FindOrCreateBulletPool()
    {
        // посик пула для пуль
        bulletPool = GetComponentInChildren<ObjectPool>();
        
        if (bulletPool == null)
        {
            // если пула нет - создаем
            GameObject poolObject = new GameObject("BulletPool");
            poolObject.transform.SetParent(transform);
            poolObject.transform.localPosition = Vector3.zero;
            
            bulletPool = poolObject.AddComponent<ObjectPool>();
            
            if (data.bulletPrefab != null)
            {
                bulletPool.prefab = data.bulletPrefab;
            }
            else
            {
                Debug.LogError("No bullet prefab in TurretData!");
            }
        }
    }
    private void Update()
    {
        if (target == null) {
            FindTarget();
            return;
        }
        

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else {
            timeUntilFire += Time.deltaTime;

            if(timeUntilFire >= 1f / bps) {
                Shoot();
                timeUntilFire = 0f;
            }
        }
        
    }

    private void Shoot() {
       /* GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet5 bulletScript = bulletObj.GetComponent<Bullet5>();
        bulletScript.SetTarget(target);
        */
       
       if (bulletPool == null)
       {
           Debug.LogError("Bullet pool not found!");
           return;
       }
       if (data.bulletPrefab == null)
       {
           Debug.LogError("Bullet prefab not set in TurretData!");
           return;
       }
        
       GameObject bulletObj = bulletPool.GetObject();
       bulletObj.transform.position = firingPoint.position;
       bulletObj.transform.rotation = Quaternion.identity;
       Bullet5 bulletScript = bulletObj.GetComponent<Bullet5>();
        
       if (bulletScript != null)
       {
           //bulletScript.SetTarget(target);
           Vector2 shootDirection = (target.position - firingPoint.position).normalized;
           bulletScript.SetDirection(shootDirection);
           bulletScript.SetDamage(turretDamage); 
           bulletScript.SetPool(bulletPool); 
           bulletScript.SetBulletType(data.bulletType);
       }
       else
       {
           Debug.LogError("Bullet prefab doesn't have Bullet5 component!");
       }
    }
    private void FindTarget() {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)
            transform.position, 0f, enemyMask);

        if (hits.Length > 0 ) {
            target = hits[0].transform;
        }
    }

    private bool CheckTargetIsInRange() {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void OnDrawGizmos()
    {
        if (data != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.range);
        }
    }
    
   /* private void RotateTowardsTarget() { 
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - 
            transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, 
            targetRotation, rotationSpeed * Time.deltaTime);
    }
    /* private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
    */
}
