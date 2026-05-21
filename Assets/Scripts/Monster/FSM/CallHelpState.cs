using UnityEngine;
using System.Collections;

public class CallHelpState : IState
{
    private MonsterController owner;

    public CallHelpState(MonsterController owner) => this.owner = owner;

    public void Enter()
    {
        var rb = owner.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        owner.SetAnimation(MonsterAnimState.Idle);
        owner.StartCoroutine(CallRoutine());
    }

    private IEnumerator CallRoutine()
    {
        yield return new WaitForSeconds(0.8f);
        SpawnReinforcements();
        yield return new WaitForSeconds(0.5f);

        owner.OnActionFinished("CallHelp");
    }

    private void SpawnReinforcements()
    {
        if (owner.data.reinforcementPrefabs == null || owner.data.reinforcementPrefabs.Count == 0) return;

        int spawnCount = Random.Range(1, 3);
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = owner.data.reinforcementPrefabs[Random.Range(0, owner.data.reinforcementPrefabs.Count)];
            if (prefab == null) continue;

            Vector2 spawnPos = (Vector2)owner.transform.position + Random.insideUnitCircle * 1.5f;
            GameObject helper = Object.Instantiate(prefab, spawnPos, Quaternion.identity);

            if (helper.TryGetComponent(out MonsterController helperCtrl))
            {
                helperCtrl.StartCoroutine(ForceCombat(helperCtrl, owner.Target));
            }
        }
    }

    private IEnumerator ForceCombat(MonsterController ctrl, Transform player)
    {
        yield return new WaitForEndOfFrame();
        if (ctrl != null && player != null) ctrl.ChangeState(new CombatState(ctrl));
    }

    public void Update() { }
    public void Exit() { }
}