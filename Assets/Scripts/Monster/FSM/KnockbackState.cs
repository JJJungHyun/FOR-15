using UnityEngine;

public class KnockbackState : IState
{
    private MonsterController owner;
    private Vector2 knockbackDir;
    private float timer;
    private float duration = 0.2f; 

    public KnockbackState(MonsterController owner, Vector2 attackerPos)
    {
        this.owner = owner;
        this.knockbackDir = ((Vector2)owner.transform.position - attackerPos).normalized;
    }

    public void Enter()
    {
        timer = 0;
        var rb = owner.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDir * 10f, ForceMode2D.Impulse);
        }
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            if (owner.Target != null) owner.ChangeState(new CombatState(owner));
            else owner.ChangeState(new IdleState(owner));
        }
    }

    public void Exit()
    {
        var rb = owner.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}