using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float range;
    private float damage;
    private Vector3 startPos;

    private EquippableItem sourceWeapon;
    private Character shooter;
    private int hitCost;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Vector3 dir, float spd, float rng, float dmg, Sprite bulletSprite = null)
    {
        direction = dir;
        speed = spd;
        range = rng;
        damage = dmg;
        startPos = transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (spriteRenderer != null && bulletSprite != null)
        {
            spriteRenderer.sprite = bulletSprite;
        }
    }

    public void SetWeaponSource(EquippableItem weapon, Character player, int cost)
    {
        sourceWeapon = weapon;
        shooter = player;
        hitCost = cost;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (Vector3.Distance(startPos, transform.position) >= range) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, transform.position);

                if (sourceWeapon != null && shooter != null)
                {
                    sourceWeapon.ConsumeDurability(hitCost, shooter);
                }
            }
            Destroy(gameObject);
        }
    }
}