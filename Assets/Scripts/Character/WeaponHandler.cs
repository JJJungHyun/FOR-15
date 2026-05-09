using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class WeaponHandler : MonoBehaviour, IWeaponAbility
{
    [Header("Attack Config")]
    [SerializeField] private float attackPreDelay = 0.1f;
    [SerializeField] private float attackDuration = 0.15f;
    [SerializeField] private float attackCooldown = 0.4f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D weaponCollider;
    private Character ownerPlayer;
    private bool isAttacking = false;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponCollider = GetComponent<BoxCollider2D>();
        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false;
    }

    public void OnAttackStart(Character player)
    {
        if (isAttacking) return;

        ownerPlayer = player;
        StartCoroutine(AttackRoutine());
    }

    public void OnAttackHold(Character player) { }
    public void OnAttackRelease(Character player) { }

    public void UpdateColliderSize()
    {
        if (spriteRenderer.sprite != null)
        {
            weaponCollider.size = spriteRenderer.sprite.bounds.size;
            weaponCollider.offset = spriteRenderer.sprite.bounds.center;
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(attackPreDelay);

        weaponCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration);

        weaponCollider.enabled = false;

        float remain = attackCooldown - attackPreDelay - attackDuration;
        if (remain > 0) yield return new WaitForSeconds(remain);

        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;
        if (other.CompareTag("Player")) return;

        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(ownerPlayer.GetAttackDamage(), transform.root.position);
        }
    }
}