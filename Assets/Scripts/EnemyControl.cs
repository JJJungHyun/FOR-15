using System.Collections;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private float maxHp;
    private float currentHp;
    private bool isDead = false;

    public GameObject[] itemPool;
    public float dropChance;

    [Header("배회")]
    public float patrolRadius;    // 배회 가능 반경
    public float moveSpeed;       // 이동 속도
    public float idleTime;        // 멈춰있는 시간

    [Header("추적")]
    public float detectRange;    // 플레이어를 감지할 범위
    public Transform player;          // 추적할 플레이어

    private Vector3 spawnPoint;        // 기준점

    void Start()
    {
        currentHp = maxHp;
        spawnPoint = transform.position;
        StartCoroutine(MainRoutine());
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
        // 플레이어가 감지 범위를 벗어날 때까지 계속 쫓아감
        // 놓치는 범위는 감지보다 조금 더 넓게 설정
        while (!isDead && player != null && Vector3.Distance(transform.position, player.position) <= detectRange + 2f) 
        {
            // 플레이어 방향으로 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * 1.5f * Time.deltaTime
            );

            yield return null;
        }

    }

    // 배회
    IEnumerator PatrolRoutine()
    {
        // 대기 (Idle)     
        float waitTime = Random.Range(idleTime * 0.5f, idleTime * 1.5f);
        float timer = 0;
        while (timer < waitTime)
        {
            if (isDead || IsPlayerDetected()) yield break; // 대기 중 플레이어 발견 시 즉시 종료
            timer += Time.deltaTime;
            yield return null;
        }

        // 목표 설정
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 targetPos = spawnPoint + new Vector3(randomCircle.x, 0, randomCircle.y);

        // 이동 (Move)
        // 목표 지점에 도착할 때까지 이 루프에서 빠져나가지 않음
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            if (isDead || IsPlayerDetected()) yield break;

            float currentSpeed = moveSpeed * 3f;

            // 위치 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                currentSpeed * Time.deltaTime
            );

            // 방향 전환 (Flip)
            float direction = targetPos.x - transform.position.x;

            if (Mathf.Abs(direction) > 0.01f)
            {
                transform.localScale = new Vector3(direction > 0 ? 1 : -1, 1, 1);
            }

            // 다음 프레임까지 대기 
                yield return null;
            }
    }

    bool IsPlayerDetected()
    {
        return player != null && Vector3.Distance(transform.position, player.position) <= detectRange;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
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
}
