using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float range;
    private float damage;
    private Vector3 startPos;

    public void Setup(Vector3 dir, float spd, float rng, float dmg)
    {
        direction = dir;
        speed = spd;
        range = rng;
        damage = dmg;
        startPos = transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log($"{collision.name}에게 {damage} 데미지!");

            if (collision.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, transform.position);
            }

            Destroy(gameObject);
        }
    }
}