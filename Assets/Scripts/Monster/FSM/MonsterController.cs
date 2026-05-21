using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour, IDamageable
{
    public MonsterData data;
    private StateMachine fsm;
    private Node btRoot;
    private SpriteRenderer spriteRenderer;
    private MonsterAnimation monsterAnim;
    private Rigidbody2D rb;

    private float currentHp;
    private bool isDead = false;
    private IDetectionCondition detectionCondition;

    public bool IsAttacking { get; set; }
    public Transform Target { get; private set; }
    public Vector3 SpawnPoint { get; private set; }

    [Header("장애물 회피 시스템 (자체 우회)")]
    [SerializeField] private LayerMask obstacleLayer; // 회피할 바위, 나무 등의 레이어
    [SerializeField] private float avoidDetectDistance = 1.2f; // 장애물을 미리 감지할 전방 거리
    [SerializeField] private float avoidRadius = 0.35f; // 몬스터의 물리적 두께 반지름

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        monsterAnim = new MonsterAnimation(GetComponent<Animator>(), spriteRenderer);

        currentHp = data.maxHp;
        SpawnPoint = transform.position;

        SetupDetectionStrategy();

        fsm = new StateMachine();
        fsm.ChangeState(new IdleState(this));
        btRoot = SetupBT();
    }

    private void SetupDetectionStrategy()
    {
        if (data.name.Contains("멧돼지") || data.name.Contains("Boar"))
            detectionCondition = new FarmingCondition();
        else
            detectionCondition = new AggressiveCondition();
    }

    private Node SetupBT()
    {
        Node attackNode = new ActionBasicAttack(this);
        if (data.attackStyle == AttackStyle.Dash) attackNode = new ActionDashAttack(this);
        else if (data.attackStyle == AttackStyle.None) return new ActionChase(this);

        return new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new ConditionIsInRange(this),
                attackNode
            }),
            new ActionChase(this)
        });
    }

    private void Update() => fsm.Update();
    public void ChangeState(IState newState) => fsm.ChangeState(newState);
    public void RunBT() => btRoot.Evaluate();

    // 레이캐스트 우회 기법이 적용된 MoveTo
    public void MoveTo(Vector3 target, float speed)
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = target;
        Vector2 moveDirection = (targetPos - currentPos).normalized;

        // 목적지와의 거리가 아주 가까우면 회피 연산을 하지 않고 직진 후 멈춤
        float distToTarget = Vector2.Distance(currentPos, targetPos);
        if (distToTarget > 0.1f)
        {
            // 1. 진행 방향 정면에 두꺼운 레이(CircleCast)를 쏘아 장애물 확인
            RaycastHit2D hit = Physics2D.CircleCast(currentPos, avoidRadius, moveDirection, avoidDetectDistance, obstacleLayer);

            if (hit.collider != null)
            {
                Vector2 alternativeDir = Vector2.zero;
                bool foundPath = false;

                // 좌우 30도, 60도, 90도 각도로 살피며 장애물이 없는 가장 빠른 길 탐색
                float[] avoidAngles = { 30f, -30f, 60f, -60f, 90f, -90f };

                foreach (float angle in avoidAngles)
                {
                    // 방향 벡터를 angle만큼 회전
                    Vector2 rotatedDir = Quaternion.Euler(0, 0, angle) * moveDirection;

                    // 해당 회전 방향으로 다시 길 체크
                    RaycastHit2D checkHit = Physics2D.CircleCast(currentPos, avoidRadius, rotatedDir, avoidDetectDistance, obstacleLayer);

                    if (checkHit.collider == null)
                    {
                        alternativeDir = rotatedDir;
                        foundPath = true;
                        break;
                    }
                }

                // 열린 길을 찾았다면 이동 방향을 우회로로 변경
                if (foundPath)
                {
                    moveDirection = alternativeDir;
                }
            }
        }

        // 2. 최종 결정된 방향으로 부드럽게 프레임 독립적 이동
        Vector3 nextPos = (Vector3)currentPos + (Vector3)(moveDirection * speed * Time.deltaTime);
        transform.position = nextPos;

        // 애니메이션 업데이트
        monsterAnim.UpdateMoveAnimation(moveDirection);
    }

    // 공격 중이거나 대기 상태일 때 제자리에서 미끄러지지 않도록 속도 초기화 함수
    public void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public bool DetectPlayer()
    {
        var col = Physics2D.OverlapCircle(transform.position, data.detectRange, LayerMask.GetMask("Player"));
        if (col != null)
        {
            if (detectionCondition.IsSatisfied(this, col.transform))
            {
                Target = col.transform;
                return true;
            }
        }
        Target = null;
        return false;
    }

    public void TakeDamage(float damage, Vector2 attackerPos)
    {
        if (isDead) return;
        currentHp -= damage;

        if (currentHp <= 0)
        {
            isDead = true;
            ChangeState(new DieState(this));
            return;
        }

        ChangeState(new KnockbackState(this, attackerPos));

        // 시퀀스 리스트를 순회하며 확률 체크
        foreach (var step in data.reactionSequence)
        {
            if (Random.Range(0f, 100f) <= step.chance)
            {
                ExecuteReaction(step.type);
                if (step.stopChain) break;
            }
        }
    }

    // 중간 단계 상태들이 종료될 때 호출할 함수
    public void OnActionFinished(string key)
    {
        var transition = data.nextActionMap.Find(x => x.triggerKey == key);
        if (transition.triggerKey != null)
        {
            ExecuteReaction(transition.nextAction);
        }
        else
        {
            ChangeState(new ReturnState(this));
        }
    }

    public void ExecuteReaction(HurtReactionType type)
    {
        switch (type)
        {
            case HurtReactionType.Flee: ChangeState(new FleeState(this)); break;
            case HurtReactionType.CallHelp: ChangeState(new CallHelpState(this)); break;
            case HurtReactionType.Counter:
                SetTargetFromDetection();
                ChangeState(new CombatState(this));
                break;
        }
    }

    private void SetTargetFromDetection()
    {
        var col = Physics2D.OverlapCircle(transform.position, data.detectRange, LayerMask.GetMask("Player"));
        if (col != null) Target = col.transform;
    }

    public void SetAnimation(MonsterAnimState state) => monsterAnim.SetAnimState(state);

    public void TryDropItem()
    {
        if (data.dropTable == null || data.dropTable.Count == 0) return;
        foreach (var dropData in data.dropTable)
        {
            if (Random.value <= dropData.dropChance)
            {
                int dropCount = Random.Range(dropData.minAmount, dropData.maxAmount + 1);
                for (int i = 0; i < dropCount; i++)
                {
                    GameObject droppedItem = Instantiate(dropData.itemPrefab, transform.position, Quaternion.identity);
                    if (droppedItem.TryGetComponent(out ItemPopUp anim)) anim.PlayDropAnimation();
                }
            }
        }
    }

    // 디버그용 기즈모 (인스펙터에서 레이가 어떻게 조준되는지 시각화)
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        // 탐지 및 순찰 반경 기즈모
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? SpawnPoint : transform.position;
        Gizmos.DrawWireSphere(center, data.patrolRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.detectRange);

        // 장애물 감지 전방 레이 실시간 기즈모
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);
    }
}