using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class EnemyCombat : MonoBehaviour, IDamageable
{
    /*[SerializeField] private GameObject[] itemPool;
    [SerializeField] private float dropChance;*/

    private Rigidbody2D rb;
    private EnemyControl movementScript;
    private bool isStunned = false;

    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private string enemyName; 

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
            TryDropItem();
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

            StartCoroutine(AttackPauseRoutine());
        }
    }

    private IEnumerator AttackPauseRoutine()
    {
        isStunned = true;

        // 이동 스크립트 비활성화
        if (movementScript != null) movementScript.enabled = false;

        yield return new WaitForSeconds(1.5f);

        // 이동 스크립트 다시 활성화 및 루틴 재시작
        if (movementScript != null)
        {
            movementScript.enabled = true;
            movementScript.StartMainRoutine(); // 재시작 함수 호출
        }

        isStunned = false;
    }

    void TryDropItem()
    {
        if (enemyData == null) return;

        var settings = enemyData.GetSettings(enemyName);
        if (settings == null) return;

        foreach (var drop in settings.drops)
        {
            float randomRoll = Random.Range(0f, 100f);

            if (randomRoll <= drop.dropRate)
            {
                int finalAmount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                if (finalAmount <= 0) continue;

                GameObject go = Instantiate(ItemPrefab, transform.position, Quaternion.identity);

                if (go.TryGetComponent(out ItemObject itemObj))
                {
                    itemObj.SetItemData(drop.itemSO, finalAmount);
                }

                if (go.TryGetComponent(out ItemPopUp anim))
                {
                    anim.PlayDropAnimation();
                }
            }
        }
    }
}