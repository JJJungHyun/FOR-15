using UnityEngine;

public class DieState : IState
{
    private MonsterController owner;
    public DieState(MonsterController owner) => this.owner = owner;

    public void Enter()
    {
        owner.SetAnimation(MonsterAnimState.Die);
        owner.TryDropItem();
        Object.Destroy(owner.gameObject, 1.0f);
    }

    public void Update() { }
    public void Exit() { }
}
