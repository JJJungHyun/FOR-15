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

    [Header("UI 시스템")]
    [SerializeField] private MonsterHPBar hpBar;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        monsterAnim = new MonsterAnimation(GetComponent<Animator>(), spriteRenderer);

        currentHp = data.maxHp;
        SpawnPoint = transform.position;

        if (hpBar != null)
        {
            hpBar.Init(data.maxHp, transform);
        }
        else
        {
            hpBar = GetComponentInChildren<MonsterHPBar>();
            if (hpBar != null) hpBar.Init(data.maxHp, transform);
        }

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

    public void MoveTo(Vector3 target, float speed)
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = target;
        Vector2 moveDirection = (targetPos - currentPos).normalized;

        float distToTarget = Vector2.Distance(currentPos, targetPos);
        if (distToTarget > 0.1f)
        {
            RaycastHit2D hit = Physics2D.CircleCast(currentPos, avoidRadius, moveDirection, avoidDetectDistance, obstacleLayer);

            if (hit.collider != null)
            {
                Vector2 alternativeDir = Vector2.zero;
                bool foundPath = false;

                float[] avoidAngles = { 30f, -30f, 60f, -60f, 90f, -90f };

                foreach (float angle in avoidAngles)
                {
                    Vector2 rotatedDir = Quaternion.Euler(0, 0, angle) * moveDirection;
                    RaycastHit2D checkHit = Physics2D.CircleCast(currentPos, avoidRadius, rotatedDir, avoidDetectDistance, obstacleLayer);

                    if (checkHit.collider == null)
                    {
                        alternativeDir = rotatedDir;
                        foundPath = true;
                        break;
                    }
                }

                if (foundPath)
                {
                    moveDirection = alternativeDir;
                }
            }
        }

        Vector3 nextPos = (Vector3)currentPos + (Vector3)(moveDirection * speed * Time.deltaTime);
        transform.position = nextPos;

        monsterAnim.UpdateMoveAnimation(moveDirection);
    }

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

        if (fsm != null && fsm.GetCurrentStateName() == "FleeState")
        {
            return false;
        }

        Target = null;
        return false;
    }

    public void TakeDamage(float damage, Vector2 attackerPos)
    {
        if (isDead) return;
        currentHp -= damage;

        if (hpBar != null)
        {
            hpBar.UpdateHP(currentHp);
        }

        if (currentHp <= 0)
        {
            isDead = true;
            ChangeState(new DieState(this));
            return;
        }

        var col = Physics2D.OverlapCircle(transform.position, data.detectRange, LayerMask.GetMask("Player"));
        if (col != null) Target = col.transform;

        ChangeState(new KnockbackState(this, attackerPos));

        foreach (var step in data.reactionSequence)
        {
            if (Random.Range(0f, 100f) <= step.chance)
            {
                ExecuteReaction(step.type);
                if (step.stopChain) break;
            }
        }
    }

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

    public void SetAnimation(MonsterAnimState state)
    {
        if (state == MonsterAnimState.Attack && data.attackStyle == AttackStyle.None) return;
        monsterAnim.SetAnimState(state);
    }

    public void PlayAttackAnimation()
    {
        if (data.attackStyle == AttackStyle.None || Target == null) return;

        Vector3 targetDir = (Target.position - transform.position).normalized;

        monsterAnim.PlayAttackAnimation(targetDir);
    }

    public void TryDropItem()
    {
        if (data.defaultItemPrefab == null || data.dropTable == null || data.dropTable.Count == 0) return;

        foreach (var dropData in data.dropTable)
        {
            if (dropData.itemData == null) continue;

            if (Random.value <= dropData.dropChance)
            {
                int dropCount = Random.Range(dropData.minAmount, dropData.maxAmount + 1);

                for (int i = 0; i < dropCount; i++)
                {
                    GameObject droppedItem = Instantiate(data.defaultItemPrefab, transform.position, Quaternion.identity);

                    if (droppedItem.TryGetComponent(out ItemObject itemObj))
                    {
                        itemObj.SetItemData(dropData.itemData, 1);
                    }

                    if (droppedItem.TryGetComponent(out ItemPopUp anim))
                    {
                        anim.PlayDropAnimation();
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        // 초록색: 순찰 범위 (Patrol Radius)
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? SpawnPoint : transform.position;
        Gizmos.DrawWireSphere(center, data.patrolRadius);
        
        // 노란색: 플레이어 감지 범위 (Detect Range)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.detectRange);

        // 빨간색: 다른 몬스터와의 회피 범위 (Avoid Radius)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);
    }
}