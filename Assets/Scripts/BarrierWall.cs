using UnityEngine;

public class BarrierWall : MonoBehaviour
{
    public TurretData data;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void InitializeFromData()
    {
        if (data == null) return;
        currentHealth = data.barrierHealth;
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            float sizeValue = data.range > 0 ? data.range : 1f;
            boxCollider.size = new Vector2(sizeValue, sizeValue);
        }
        if (spriteRenderer != null)
        {
            if (data.worldSprite != null) spriteRenderer.sprite = data.worldSprite;
            spriteRenderer.color = data.towerColor;
        }
        gameObject.name = data.name + " (Barrier)";
        
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
            boxCollider.enabled = true;
        }
        
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            DestroyBarrier();
        }
    }

    private void DestroyBarrier()
    {
        Destroy(gameObject);
    }
}