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

    public void MoveTo(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        monsterAnim.UpdateMoveAnimation(direction);
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
                if (step.stopChain) break; // 이번 행동이 사슬을 끊는다면 중단
            }
        }
    }

    // 중간 단계 상태들이 종료될 때 호출할 함수
    public void OnActionFinished(string key)
    {
        // 데이터 맵에서 다음 목적지를 찾음
        var transition = data.nextActionMap.Find(x => x.triggerKey == key);
        if (transition.triggerKey != null)
        {
            ExecuteReaction(transition.nextAction);
        }
        else
        {
            // 매핑이 없으면 기본적으로 복귀
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

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? SpawnPoint : transform.position;
        Gizmos.DrawWireSphere(center, data.patrolRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.detectRange);
    }
}