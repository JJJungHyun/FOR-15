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

        if (data.hurtReaction == HurtReactionType.CallHelp)
        {
            if (Random.value < data.callHelpChance)
            {
                ChangeState(new CallHelpState(this));
            }
            else
            {
                ChangeState(new FleeState(this));
            }
        }
        else
        {
            switch (data.hurtReaction)
            {
                case HurtReactionType.Flee:
                    ChangeState(new FleeState(this));
                    break;
                case HurtReactionType.Counter:
                    var col = Physics2D.OverlapCircle(transform.position, data.detectRange, LayerMask.GetMask("Player"));
                    if (col != null) Target = col.transform;
                    ChangeState(new CombatState(this));
                    break;
            }
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