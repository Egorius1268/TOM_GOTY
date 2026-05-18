using UnityEngine;

public class BulletModifier : MonoBehaviour
{
    public ModifierTowerData data; 
    [Header("Effect")]
    public BulletType newBulletType; 
    public bool overrideType = true; // true = заменить полностью, false = добавить эффект

    [Header("Trigger")]
    public LayerMask bulletLayer; 
    
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        if (data != null)
        {
            InitializeFromData();
        }
    }
    
    public void InitializeFromData()
    {
        if (data == null) return;

        
        if (boxCollider != null)
        {
            boxCollider.size = new Vector2(data.triggerRadius, data.triggerRadius);
        }

        // Подгружаем визуал
        if (spriteRenderer != null)
        {
            if (data.icon != null) spriteRenderer.sprite = data.worldSprite;
            spriteRenderer.color = data.towerColor;
        }
        
        gameObject.name = data.name + " (Modifier)";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Bullet5 bullet = other.GetComponent<Bullet5>();
        if (bullet == null) return;

        
        if ((bulletLayer.value & (1 << other.gameObject.layer)) == 0) return;

        
        if (data != null && data.bulletType != null)
        {
            bullet.SetBulletType(data.bulletType); 
        }
    }
}