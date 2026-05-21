using UnityEngine;

public class PatrolState : IState
{
    private MonsterController owner;
    private Vector3 targetPos;
    private float timer;

    public PatrolState(MonsterController o) => owner = o;

    public void Enter()
    {
        timer = 0;
        SetNewTarget();
    }

    public void Update()
    {
        if (owner.DetectPlayer() && owner.data.disposition == MonsterDisposition.Aggressive)
        {
            owner.ChangeState(new CombatState(owner));
            return;
        }

        float distFromSpawn = Vector3.Distance(owner.transform.position, owner.SpawnPoint);
        if (distFromSpawn > owner.data.patrolRadius)
        {
            SetNewTarget();
            return;
        }

        owner.MoveTo(targetPos, owner.data.walkSpeed);

        timer += Time.deltaTime;
        if (timer >= owner.data.moveTime || Vector3.Distance(owner.transform.position, targetPos) < 0.1f)
        {
            owner.ChangeState(new IdleState(owner));
        }
    }

    private void SetNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * owner.data.patrolRadius;
        targetPos = owner.SpawnPoint + new Vector3(randomOffset.x, randomOffset.y, 0);
    }

    public void Exit() { }
}