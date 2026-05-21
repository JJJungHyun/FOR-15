using UnityEngine;

public class ReturnState : IState
{
    private MonsterController owner;
    public ReturnState(MonsterController owner) => this.owner = owner;

    public void Enter() { }

    public void Update()
    {
        owner.MoveTo(owner.SpawnPoint, owner.data.walkSpeed);

        if (Vector3.Distance(owner.transform.position, owner.SpawnPoint) < 0.1f)
        {
            owner.ChangeState(new IdleState(owner));
        }
    }
    public void Exit() { }
}