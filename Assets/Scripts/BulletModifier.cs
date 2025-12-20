using UnityEngine;

public class BulletModifier : MonoBehaviour
{
    [Header("Effect")]
    public BulletType newBulletType; 
    public bool overrideType = true; // true = заменить полностью, false = добавить эффект

    [Header("Trigger")]
    public LayerMask bulletLayer; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Bullet5 bullet = other.GetComponent<Bullet5>();
        if (bullet == null) return;

        
        if ((bulletLayer.value & (1 << other.gameObject.layer)) == 0) return;

        
        if (overrideType && newBulletType != null)
        {
            bullet.SetBulletType(newBulletType);
        }
       
    }
}