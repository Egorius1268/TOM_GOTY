using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public TurretData data;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeFromData()
    {
        if (data == null) return;
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(data.range, data.range);
        }

        if (spriteRenderer != null)
        {
            if (data.worldSprite != null) spriteRenderer.sprite = data.worldSprite;
            spriteRenderer.color = data.towerColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (data == null) return;

        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null && data.damage > 0)
            {
                enemyHealth.TakeDamage((int)data.damage);
            }
            EnemyEffects enemyEffects = other.GetComponent<EnemyEffects>();
            if (enemyEffects != null)
            {
                float duration = data.effectDuration > 0 ? data.effectDuration : 3f; 
                if (data.trapEffectState != null)
                {
                    string stateName = data.trapEffectState.ToString().ToLower();
                    if (stateName.Contains("frost") || stateName.Contains("slow"))
                    {
                        enemyEffects.ApplySlowEffect(0.5f, duration);
                    }
                    else if (stateName.Contains("fire") || stateName.Contains("ignite") || stateName.Contains("burn"))
                    {
                        float dps = data.damage > 0 ? data.damage / duration : 2f;
                        enemyEffects.ApplyBurnEffect(dps, duration);
                    }
                    else if (stateName.Contains("poison"))
                    {
                        float totalPoisonDamage = data.damage > 0 ? data.damage * 2f : 15f;
                        enemyEffects.ApplyPoisonEffect(totalPoisonDamage, duration);
                        Debug.Log($"Ловушка ОТРАВИЛА {other.name}");
                    }
                }
            }
        }
    }
}