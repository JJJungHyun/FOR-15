using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float range;
    private float damage;
    private Vector3 startPos;

    // 명중 시 내구도 차감을 위한 백업 데이터
    private EquippableItem sourceWeapon;
    private Character shooter;
    private int hitCost;

    // 외형 변경을 위한 SpriteRenderer 컴포넌트 참조
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 🌟 파라미터에 탄환 스프라이트(bulletSprite) 추가
    public void Setup(Vector3 dir, float spd, float rng, float dmg, Sprite bulletSprite = null)
    {
        direction = dir;
        speed = spd;
        range = rng;
        damage = dmg;
        startPos = transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 🌟 전달받은 탄환 스프라이트가 있다면 투사체 이미지를 변경
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