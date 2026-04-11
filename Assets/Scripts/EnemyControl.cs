using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyControl : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("배회")]
    public float patrolRadius;    // 배회 가능 반경
    public float moveSpeed;       // 이동 속도
    public float idleTime;        // 멈춰있는 시간

    [Header("추적")]
    public float detectRange;    // 플레이어를 감지할 범위
    private Transform player;    // 추적할 플레이어

    private Vector3 spawnPoint;  // 기준점

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spawnPoint = transform.position;
        StartCoroutine(MainRoutine());
    }

    IEnumerator MainRoutine()
    {
        while (true)
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

        while (player != null)
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
            if (IsPlayerDetected()) yield break;
            timer += Time.deltaTime;
            yield return null;
        }

        // 목표 지점 설정
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        Vector3 targetPos = spawnPoint + new Vector3(randomPoint.x, randomPoint.y, 0);

        // 이동
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            if (IsPlayerDetected()) yield break;

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


    void FlipSprite(float targetX)
    {
        float directionX = targetX - transform.position.x;
        if (Mathf.Abs(directionX) > 0.05f)
        {
            // 적이 오른쪽을 보고 있는 스프라이트 기준
            spriteRenderer.flipX = (directionX < 0);
        }
    }
}
