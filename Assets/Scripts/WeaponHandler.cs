using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class WeaponHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D weaponCollider;
    private float damage;
    private bool isAttacking = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponCollider = GetComponent<BoxCollider2D>();

        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false;
    }

    public void UpdateColliderSize()
    {
        if (spriteRenderer.sprite != null)
        {
            weaponCollider.size = spriteRenderer.sprite.bounds.size;
            weaponCollider.offset = spriteRenderer.sprite.bounds.center;
        }
    }

    public void EnableAttack(float attackDamage)
    {
        damage = attackDamage;
        isAttacking = true;
        weaponCollider.enabled = true;
    }

    public void DisableAttack()
    {
        isAttacking = false;
        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;
        if (other.CompareTag("Player")) return;

        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}