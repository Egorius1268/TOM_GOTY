using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;            // Время жизни пули (сек)
    public ObjectPool pool;                // Пул объектов (может быть null)
    public float damage = 10f;             // Урон

    private Rigidbody2D rb;
    private Coroutine lifeCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // Сбрасываем скорость (на случай повторного использования из пула)
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Запускаем таймер уничтожения
        lifeCoroutine = StartCoroutine(ReturnAfter(lifeTime));
    }

    void OnDisable()
    {
        // Останавливаем корутину, чтобы избежать ошибок
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }
    }

    IEnumerator ReturnAfter(float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool();
    }

    // Столкновение с коллайдером (если пуля имеет Collider2D + Rigidbody2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryApplyDamage(collision.gameObject);
        ReturnToPool();
    }

    // Триггер (если пуля использует триггер вместо физики)
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryApplyDamage(other.gameObject);
        ReturnToPool();
    }

    // Общая логика нанесения урона
    private void TryApplyDamage(GameObject other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // Можно добавить визуальные эффекты, звук и т.д.
        }
    }

    // Возврат в пул или деактивация
    public void ReturnToPool()
    {
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (pool != null)
        {
            pool.ReturnObject(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Метод для установки цели (если пуля самонаводящаяся или имеет направление)
    // В твоём случае — просто задаём направление при выстреле из башни
    public void SetDirection(Vector2 direction, float speed)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
    }
}