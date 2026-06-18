using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class EnemyCombat : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private EnemyControl movementScript;
    private bool isStunned = false;
    [SerializeField] private MonsterData monsterData;

    private MonsterHPBar hpBar;
    private float currentHp;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 12f;
    [SerializeField] private float knockbackStunDuration = 0.15f;

    [Header("Events (Optional)")]
    public UnityEvent OnKnockbackBegin;
    public UnityEvent OnKnockbackDone;

    private float lastAttackTime;

    private void Awake()
    {
        currentHp = monsterData.maxHp;
        rb = GetComponent<Rigidbody2D>();
        movementScript = GetComponent<EnemyControl>();
        hpBar = GetComponentInChildren<MonsterHPBar>();
    }

    public void TakeDamage(float damage, Vector2 attackerPosition)
    {
        currentHp -= damage;

        if (hpBar != null && monsterData != null)
        {
            hpBar.UpdateHP(currentHp);
        }

        StopAllCoroutines();
        StartCoroutine(KnockbackRoutine(attackerPosition));

        if (currentHp <= 0)
        {
            //DropItems();
            Destroy(gameObject);
        }
    }
    /*private void DropItems()
    {
        // 데이터나 드롭 테이블이 비어있다면 실행하지 않음
        if (monsterData == null || monsterData.dropTable == null) return;

        foreach (DropItemData dropData in monsterData.dropTable)
        {
            // 프리팹이 연결되어 있지 않다면 패스
            if (dropData.itemPrefab == null) continue;

            // 0.0 ~ 1.0 사이의 랜덤값을 뽑아 드롭 확률(dropChance)과 비교
            float randomRoll = Random.Range(0f, 1f);
            if (randomRoll <= dropData.dropChance)
            {
                // 최소(minAmount) ~ 최대(maxAmount) 사이의 랜덤 수량 결정
                int dropAmount = Random.Range(dropData.minAmount, dropData.maxAmount + 1);

                for (int i = 0; i < dropAmount; i++)
                {
                    // 늑대의 현재 위치에 아이템 프리팹 생성
                    GameObject droppedItem = Instantiate(dropData.itemPrefab, transform.position, Quaternion.identity);

                    ItemPopUp anim = droppedItem.GetComponent<ItemPopUp>();

                    // 4. 애니메이션 실행
                    if (anim != null)
                    {
                        anim.PlayDropAnimation();
                    }
                }
            }
        }
    }*//*

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
}