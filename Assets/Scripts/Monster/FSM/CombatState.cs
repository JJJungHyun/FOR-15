using UnityEngine;

public class CombatState : IState
{
    private MonsterController owner;
    public CombatState(MonsterController o) => owner = o;

    public void Enter() { }

    public void Update()
    {
        if (owner.IsAttacking) return;

        float distFromSpawn = Vector3.Distance(owner.transform.position, owner.SpawnPoint);
        if (distFromSpawn > owner.data.returnRange)
        {
            owner.ChangeState(new ReturnState(owner));
            return;
        }

        if (owner.Target == null)
        {
            owner.ChangeState(new ReturnState(owner));
            return;
        }

        owner.RunBT();
    }
    public void Exit() { }
}