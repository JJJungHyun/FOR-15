using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float currentHp;
    private bool isDead = false;

    [SerializeField] private GameObject[] itemPool;
    [SerializeField] private float dropChance;

    [SerializeField] private TextMeshPro hpText;
    private SpriteRenderer spriteRenderer;

    [Header("배회")]
    public float patrolRadius;    // 배회 가능 반경
    public float moveSpeed;       // 이동 속도
    public float idleTime;        // 멈춰있는 시간

    [Header("추적")]
    public float detectRange;    // 플레이어를 감지할 범위
    private Transform player;    // 추적할 플레이어

    [Header("공격")]
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;

    private Vector3 spawnPoint;  // 기준점
    private float lastAttackTime;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHp = maxHp;
        UpdateHPText();
        spawnPoint = transform.position;
        StartCoroutine(MainRoutine());
    }
    private void LateUpdate()
    {
        if (hpText != null)
        {
            Vector3 currentScale = hpText.transform.localScale;

            hpText.transform.rotation = Quaternion.identity;

            float parentXScale = transform.lossyScale.x;
            if (parentXScale < 0)
                hpText.transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
            else
                hpText.transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);
        }
    }

    IEnumerator MainRoutine()
    {
        while (!isDead)
        {
            // 플레이어가 주변에 있는지 체크
            if (IsPlayerDetected())
            {
                // 추적
                yield return StartCoroutine(ChaseRoutine());
            }
            else
            {
                // 배회
                yield return StartCoroutine(PatrolRoutine());
            }

            yield return null;
        }
    }

    // 추적
    IEnumerator ChaseRoutine()
    {
        float stopDistance = 0.2f;

        while (!isDead && player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);

            if (dist > detectRange + 2f) yield break;

            // 플레이어 방향 바라보기 (Flip)
            FlipSprite(player.position.x);

            if (dist > stopDistance)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    moveSpeed * 1.2f * Time.deltaTime
                );
            }
            yield return null;
        }
    }

    // 배회
    IEnumerator PatrolRoutine()
    {
        // 대기
        float waitTime = Random.Range(idleTime * 0.5f, idleTime * 1.5f);
        float timer = 0;
        while (timer < waitTime)
        {
            if (isDead || IsPlayerDetected()) yield break;
            timer += Time.deltaTime;
            yield return null;
        }

        // 목표 지점 설정
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        Vector3 targetPos = spawnPoint + new Vector3(randomPoint.x, randomPoint.y, 0);

        // 이동
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            if (isDead || IsPlayerDetected()) yield break;

            FlipSprite(targetPos.x);

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
    
    bool IsPlayerDetected()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectRange);

        foreach (var col in colliders)
        {
            // 그 중 태그가 "Player"인 것이 있다면
            if (col.CompareTag("Player"))
            {
                player = col.transform; // 발견한 플레이어를 타겟으로 지정
                return true;
            }
        }

        return false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHp -= damage;
        UpdateHPText();

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void FlipSprite(float targetX)
    {
        float directionX = targetX - transform.position.x;
        if (Mathf.Abs(directionX) > 0.05f)
        {
            // 적이 오른쪽을 보고 있는 스프라이트 기준
            spriteRenderer.flipX = (directionX < 0);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();
        TryDropItem();
        Destroy(gameObject);
    }

    void TryDropItem()
    {
        if (itemPool.Length == 0) return;

        if (Random.value <= dropChance)
        {
            // 배열 중에서 랜덤하게 하나 선택
            int randomIndex = Random.Range(0, itemPool.Length);
            GameObject selectedItem = itemPool[randomIndex];

            Instantiate(selectedItem, transform.position, Quaternion.identity);
        }
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
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                TryAttack(damageable);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out IDamageable damageable))
        {
            TryAttack(damageable);
        }
    }

    private void TryAttack(IDamageable target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            target.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}
