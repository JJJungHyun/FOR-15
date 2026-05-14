using UnityEngine;

public class ActionChase : Node
{
    private MonsterController owner;
    public ActionChase(MonsterController o) => owner = o;
    public override NodeState Evaluate()
    {
        if (owner.Target == null) return NodeState.Failure;
        owner.MoveTo(owner.Target.position, owner.data.chaseSpeed);
        return NodeState.Running;
    }
}