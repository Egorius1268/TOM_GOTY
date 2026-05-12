using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int maxHealth = 8;
    [SerializeField] private int currentHealth;
    [SerializeField] FloatingHealthBar healthBar;
    
    public EnemyData enemyData;
    
    
    private bool isDestroyed = false;
    private void Awake(){
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }
    void Start(){
        
        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
            maxHealth = enemyData.maxHealth; 
        }
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }
    
    public void TakeDamage (int dmg)
    {
        currentHealth -= dmg;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
        if(currentHealth <= 0 && !isDestroyed) {
            isDestroyed = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMoney(enemyData.moneyReward);
            }
            Destroy(gameObject);
            EnemySpawner.onEnemyDestroy.Invoke();
        }
    }
}
