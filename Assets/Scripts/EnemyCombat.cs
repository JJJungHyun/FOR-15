using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyCombat : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private EnemyControl movementScript;
    private MonsterHPBar hpBar;
    private float currentHp;
    private float lastAttackTime;
    private bool isDead;
    private bool isStunned;

    [SerializeField] private MonsterData monsterData;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 12f;
    [SerializeField] private float knockbackStunDuration = 0.15f;

    [Header("Respawn")]
    [SerializeField] private bool respawnEnabled = true;
    [SerializeField] private float respawnDelay = 30f;

    [Header("Events (Optional)")]
    public UnityEvent OnKnockbackBegin;
    public UnityEvent OnKnockbackDone;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<EnemyControl>();
        hpBar = GetComponentInChildren<MonsterHPBar>();

        currentHp = monsterData != null ? monsterData.maxHp : 1f;

        if (hpBar != null)
        {
            hpBar.Init(currentHp, transform);
        }
    }

    public void TakeDamage(float damage, Vector2 attackerPosition)
    {
        if (isDead) return;

        currentHp -= damage;

        if (hpBar != null)
        {
            hpBar.UpdateHP(currentHp);
        }

        if (currentHp <= 0f)
        {
            Die();
            return;
        }

        StopAllCoroutines();
        isStunned = false;
        StartCoroutine(KnockbackRoutine(attackerPosition));
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();

        if (respawnEnabled)
        {
            RespawnManager.Schedule(gameObject, respawnDelay);
        }

        Destroy(gameObject);
    }

    private IEnumerator KnockbackRoutine(Vector2 attackerPos)
    {
        OnKnockbackBegin?.Invoke();
        if (movementScript != null) movementScript.enabled = false;

        Vector2 knockbackDir = ((Vector2)transform.position - attackerPos).normalized;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(knockbackStunDuration);

        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (!isDead && movementScript != null) movementScript.enabled = true;
        OnKnockbackDone?.Invoke();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isDead) CheckAndAttack(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isDead) CheckAndAttack(collision.gameObject);
    }

    private void CheckAndAttack(GameObject targetObj)
    {
        if (monsterData == null) return;

        if (targetObj.CompareTag("Player") && targetObj.TryGetComponent(out IDamageable damageable))
        {
            TryAttack(damageable);
        }
    }

    private void TryAttack(IDamageable target)
    {
        if (monsterData == null || isStunned) return;

        if (Time.time >= lastAttackTime + monsterData.attackCooldown)
        {
            target.TakeDamage(monsterData.attackDamage, transform.position);
            lastAttackTime = Time.time;

            StartCoroutine(AttackPauseRoutine());
        }
    }

    private IEnumerator AttackPauseRoutine()
    {
        isStunned = true;
        if (movementScript != null) movementScript.enabled = false;

        yield return new WaitForSeconds(1.5f);

        if (!isDead && movementScript != null)
        {
            movementScript.enabled = true;
            movementScript.StartMainRoutine();
        }

        isStunned = false;
    }
}