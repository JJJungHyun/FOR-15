using UnityEngine;
using System.Collections;

public class ActionDashAttack : Node
{
    private MonsterController owner;
    private float lastAttackTime;

    public ActionDashAttack(MonsterController o) => owner = o;

    public override NodeState Evaluate()
    {
        if (owner.IsAttacking) return NodeState.Running;

        if (Time.time >= lastAttackTime + owner.data.attackCooldown)
        {
            owner.StartCoroutine(DashRoutine());
            lastAttackTime = Time.time;
            return NodeState.Success;
        }
        return NodeState.Failure;
    }

    private IEnumerator DashRoutine()
    {
        owner.IsAttacking = true;

        owner.PlayAttackAnimation();
        yield return new WaitForSeconds(0.7f);

        float elapsed = 0;
        float duration = 0.4f;
        Vector3 targetPos = owner.Target.position;

        while (elapsed < duration)
        {
            owner.MoveTo(targetPos, owner.data.chaseSpeed * 2f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        owner.Target?.GetComponent<IDamageable>()?.TakeDamage(owner.data.attackDamage, owner.transform.position);

        yield return new WaitForSeconds(0.8f);

        owner.SetAnimation(MonsterAnimState.Idle);
        owner.IsAttacking = false;
    }
}