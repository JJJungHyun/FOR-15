using UnityEngine;

public class IdleState : IState
{
    private MonsterController owner;
    private float idleTimer;
    private float targetIdleTime;

    public IdleState(MonsterController owner) => this.owner = owner;

    public void Enter()
    {
        idleTimer = 0f;
        targetIdleTime = owner.data.idleTime;
        owner.SetAnimation(MonsterAnimState.Idle);
    }

    public void Update()
    {
        if (owner.DetectPlayer() && owner.data.disposition == MonsterDisposition.Aggressive)
        {
            owner.ChangeState(new CombatState(owner));
            return;
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= targetIdleTime)
        {
            owner.ChangeState(new PatrolState(owner));
        }
    }
    public void Exit() { }
}