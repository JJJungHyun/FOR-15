using System.Collections;
using UnityEngine;

public class Rabbit : EnemyBase
{
    [Header("Rabbit Behavior Settings")]
    [SerializeField] private float idleDuration = 1.5f;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float fleeDuration = 3f;
    [Range(0, 100)]
    [SerializeField] private int howlProbability = 50; 

    private Vector3 spawnPoint;
    private bool isFleeing = false;

    protected override void Awake()
    {
        base.Awake();
        spawnPoint = transform.position;
        StartCoroutine(MainRoutine());
    }

    IEnumerator MainRoutine()
    {
        while (true)
        {
            if (!isFleeing)
            {
                yield return StartCoroutine(PatrolRoutine());
            }
            yield return null;
        }
    }

    IEnumerator PatrolRoutine()
    {
        yield return new WaitForSeconds(idleDuration);

        Vector2 target = (Vector2)spawnPoint + Random.insideUnitCircle * patrolRadius;
        float timer = 0;

        while (timer < moveDuration && !isFleeing)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, settings.moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;

            if (Vector2.Distance(transform.position, target) < 0.1f) break;
            yield return null;
        }
    }

    public override void TakeDamage(float damage, Vector2 attackerPosition)
    {
        base.TakeDamage(damage, attackerPosition);
        if (currentHp > 0)
        {
            StopAllCoroutines(); 
            StartCoroutine(FleeRoutine(attackerPosition));
        }
    }

    IEnumerator FleeRoutine(Vector2 attackerPos)
    {
        isFleeing = true;

        if (Random.Range(0, 100) < howlProbability)
            Debug.Log("<color=yellow>울부짖기 : 주변 몬스터 호출</color>");

        Vector2 fleeDir = ((Vector2)transform.position - attackerPos).normalized;
        float timer = 0;

        while (timer < fleeDuration)
        {
            transform.Translate(fleeDir * settings.moveSpeed * 1.5f * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isFleeing = false;
        StartCoroutine(MainRoutine()); 
    }
}