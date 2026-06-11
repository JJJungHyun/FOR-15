using UnityEngine;
using System.Collections;

public class ActionBasicAttack : Node
{
    private MonsterController owner;
    private float lastAttackTime;

    public ActionBasicAttack(MonsterController o) => owner = o;

    public override NodeState Evaluate()
    {
        if (owner.IsAttacking) return NodeState.Running;

        if (Time.time >= lastAttackTime + owner.data.attackCooldown)
        {
            owner.StartCoroutine(AttackRoutine());
            lastAttackTime = Time.time;
            return NodeState.Success;
        }
        return NodeState.Failure;
    }

    private IEnumerator AttackRoutine()
    {
        owner.IsAttacking = true;

        owner.PlayAttackAnimation();

        yield return new WaitForSeconds(0.5f);
        owner.Target?.GetComponent<IDamageable>()?.TakeDamage(owner.data.attackDamage, owner.transform.position);

        yield return new WaitForSeconds(0.5f);

        owner.SetAnimation(MonsterAnimState.Idle);
        owner.IsAttacking = false;
    }
}