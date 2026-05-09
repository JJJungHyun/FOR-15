using UnityEngine;
using System.Collections;

public class Boar : MonoBehaviour
{
    [Header("Patrol")]
    public float patrolSpeed = 2.5f;
    public float patrolRadius = 4f;
    public float patrolChangeInterval = 2f;
    public float idleTime = 1.2f;

    [Header("Detection / Attack")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    [Header("Charge")]
    public float chargeSpeed = 18f;
    public float chargeDuration = 0.5f;
    public float chargeReadyTime = 0.5f;
    public float chargeCooldown = 2f;

    [Header("Return")]
    public float returnSpeed = 4f;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color readyColor = Color.red;

    private Color originColor;
    private Rigidbody2D rb;

    private Vector2 spawnPoint;
    private Vector2 patrolTarget;

    private Transform player;

    private bool isCharging = false;
    private bool canCharge = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originColor = spriteRenderer.color;

        spawnPoint = transform.position;
        patrolTarget = GetRandomPatrolPoint();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        StartCoroutine(AI());
    }

    // =========================
    // 🧠 MAIN AI LOOP
    // =========================
    IEnumerator AI()
    {
        while (true)
        {
            if (player != null && IsPlayerDetected())
            {
                if (IsPlayerInAttackRange())
                {
                    yield return ChargeRoutine();
                }
                else
                {
                    // 공격 사거리 밖이면 접근 (단순 대기/이동)
                    rb.linearVelocity = Vector2.zero;
                }
            }
            else
            {
                if (!isCharging)
                {
                    if (IsOutOfRange())
                    {
                        yield return ReturnToCenter();
                    }
                    else
                    {
                        yield return Patrol();
                        yield return Idle();
                    }
                }
            }

            yield return null;
        }
    }

    // =========================
    // 🚶 PATROL
    // =========================
    IEnumerator Patrol()
    {
        float t = 0f;

        while (t < patrolChangeInterval && !IsPlayerDetected())
        {
            Vector2 dir = (patrolTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * patrolSpeed;

            if (Vector2.Distance(transform.position, patrolTarget) < 0.3f)
                patrolTarget = GetRandomPatrolPoint();

            t += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator Idle()
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(idleTime);
    }

    // =========================
    // ⚡ CHARGE (ATTACK)
    // =========================
    IEnumerator ChargeRoutine()
    {
        if (!canCharge) yield break;

        isCharging = true;
        canCharge = false;

        rb.linearVelocity = Vector2.zero;

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        spriteRenderer.color = readyColor;
        yield return new WaitForSeconds(chargeReadyTime);
        spriteRenderer.color = originColor;

        float t = 0f;

        while (t < chargeDuration)
        {
            rb.linearVelocity = dir * chargeSpeed;
            t += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isCharging = false;

        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }

    // =========================
    // 🧭 RETURN TO SPAWN AREA
    // =========================
    IEnumerator ReturnToCenter()
    {
        while (isCharging)
            yield return null;

        while (Vector2.Distance(transform.position, spawnPoint) > 0.2f)
        {
            Vector2 dir = (spawnPoint - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * returnSpeed;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    // =========================
    // 📌 DETECTION
    // =========================
    bool IsPlayerDetected()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    bool IsPlayerInAttackRange()
    {
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    bool IsOutOfRange()
    {
        return Vector2.Distance(transform.position, spawnPoint) > patrolRadius * 1.5f;
    }

    Vector2 GetRandomPatrolPoint()
    {
        return spawnPoint + Random.insideUnitCircle * patrolRadius;
    }

    private void OnDrawGizmos()
    {
        // 스폰 기준점 (순찰 중심)
        Vector3 center = Application.isPlaying ? spawnPoint : transform.position;

        // =========================
        // 🟢 Patrol Radius
        // =========================
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, patrolRadius);

        // =========================
        // 🔵 Detection Range
        // =========================
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // =========================
        // 🔴 Attack Range
        // =========================
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}