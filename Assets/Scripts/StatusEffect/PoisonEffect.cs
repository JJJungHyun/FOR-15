using UnityEngine;

public class PoisonEffect : ConditionEffect
{
    private float damagePerSecond;
    private float tickTimer;

    public PoisonEffect(Character owner, float duration, float damage, object source)
        : base(owner, duration, source)
    {
        Name = "Poison";
        damagePerSecond = damage;
    }

    public override void OnStart()
    {
    }

    public override void OnUpdate()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer >= 1f)
        {
            owner.Health.CurrentValue -= damagePerSecond;
            tickTimer = 0;
        }
    }

    public override void OnEnd()
    {
        Debug.Log("독 효과 종료");
    }
}