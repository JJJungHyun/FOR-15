using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class EnemyCombat : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private EnemyControl movementScript;

    [Header("Stats")]
    [SerializeField] private float maxHp = 50f;
    private float currentHp;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Visual Debug")]
    [SerializeField] private TextMeshPro hpText;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 12f;
    [SerializeField] private float knockbackStunDuration = 0.15f;

    [Header("Events (Optional)")]
    public UnityEvent OnKnockbackBegin;
    public UnityEvent OnKnockbackDone;

    private float lastAttackTime;

    private void Awake()
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<EnemyControl>();
        UpdateHPText();
    }

    private void LateUpdate()
    {
        if (hpText != null)
        {
            hpText.transform.rotation = Quaternion.identity;
            float parentXScale = transform.lossyScale.x;
            Vector3 currentScale = hpText.transform.localScale;

            if (parentXScale < 0)
                hpText.transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
            else
                hpText.transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }

    public void TakeDamage(float damage, Vector2 attackerPosition)
    {
        currentHp -= damage;
        UpdateHPText();

        StopAllCoroutines();
        StartCoroutine(KnockbackRoutine(attackerPosition));

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
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

        if (movementScript != null) movementScript.enabled = true;
        OnKnockbackDone?.Invoke();
    }

    private void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHp} / {maxHp}";
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckAndAttack(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckAndAttack(collision.gameObject);
    }

    private void CheckAndAttack(GameObject targetObj)
    {
        if (targetObj.CompareTag("Player") && targetObj.TryGetComponent(out IDamageable damageable))
        {
            TryAttack(damageable);
        }
    }

    private void TryAttack(IDamageable target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            target.TakeDamage(damage, transform.position);
            lastAttackTime = Time.time;
        }
    }
}