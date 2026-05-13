using System;
using UnityEngine;

public class Buff_DebuffZones : MonoBehaviour
{
    public float damageMultiplayer = 1f;
    public float rangeMultiplayer = 1f;
    public float bpsMultiplayer = 1f;
    public float zoneRadius;
    
    public Color zoneColor = new Color(1, 1, 1, 0.25f);

    private Sprite lastSprite;
    
    private void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = zoneColor;
        }
        
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = zoneRadius;
        }
        
        CorrectSpriteSize();
    }
    
    
    private void OnValidate()
    {
        CorrectSpriteSize();
    }

    private void CorrectSpriteSize()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = zoneRadius;
        }
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.drawMode = SpriteDrawMode.Sliced; 
            float diameter = zoneRadius * 2f;
            sr.size = new Vector2(diameter, diameter);
        }
        transform.localScale = Vector3.one; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = zoneColor;
        Gizmos.DrawSphere(transform.position, zoneRadius);
    }
}
