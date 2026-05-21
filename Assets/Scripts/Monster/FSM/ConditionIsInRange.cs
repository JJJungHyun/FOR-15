using UnityEngine;

public class ConditionIsInRange : Node
{
    private MonsterController owner;
    public ConditionIsInRange(MonsterController o) => owner = o;
    public override NodeState Evaluate()
    {
        if (owner.Target == null) return NodeState.Failure;
        float dist = Vector2.Distance(owner.transform.position, owner.Target.position);
        return dist <= owner.data.attackRange ? NodeState.Success : NodeState.Failure;
    }
}