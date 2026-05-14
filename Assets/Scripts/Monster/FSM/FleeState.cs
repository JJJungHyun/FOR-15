using UnityEngine;

public class FleeState : IState
{
    private MonsterController owner;

    public FleeState(MonsterController owner) => this.owner = owner;

    public void Enter() { }

    public void Update()
    {
        bool isPlayerNear = owner.DetectPlayer();

        if (!isPlayerNear)
        {
            owner.ChangeState(new ReturnState(owner));
            return;
        }

        if (owner.Target == null)
        {
            owner.ChangeState(new ReturnState(owner));
            return;
        }

        Vector3 fleeDir = (owner.transform.position - owner.Target.position).normalized;
        Vector3 targetPos = owner.transform.position + fleeDir;

        owner.MoveTo(targetPos, owner.data.chaseSpeed);
    }

    public void Exit() { }
}