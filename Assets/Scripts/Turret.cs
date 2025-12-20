using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Turret : MonoBehaviour
{
    [Header("References")]
   // [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 60f;
    //[SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float bps = 1f;

    private Transform target;
    private float timeUntilFire;
    private void Update()
    {
        if (target == null) {
            Debug.LogError("starting finding target");
            FindTarget();
            return;
        }

        

        if (!CheckTargetIsInRange())
        {
            target = null;
            Debug.LogError("No target found");
        }
        else {
            Debug.Log("Target found");
            timeUntilFire += Time.deltaTime;

            if(timeUntilFire >= 1f / bps) {
                Shoot();
                timeUntilFire = 0f;
            }
        }
        
    }

    public void Shoot() {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet2 bulletScript = bulletObj.GetComponent<Bullet2>();
        bulletScript.SetTarget(target);
    }
    public void FindTarget() 
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, targetingRange, enemyMask);

        if (colliders.Length > 0) 
        {
            target = colliders[0].transform;
        }
    }

    private bool CheckTargetIsInRange() {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

   /* private void RotateTowardsTarget() { 
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - 
            transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, 
            targetRotation, rotationSpeed * Time.deltaTime);
    }
    */
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
    
}
