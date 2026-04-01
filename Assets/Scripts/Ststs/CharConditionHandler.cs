using UnityEngine;

public class CharConditionHandler : MonoBehaviour, IDamageable
{
    private Character owner;

    [Header("Hunger Settings")]
    [SerializeField] private float hungerDecayRate = 0.5f;       // 초당 허기 감소량
    [SerializeField] private float healthRegenRate = 0.2f;      // 배부를 때 초당 회복량
    [SerializeField] private float starvationDamageRate = 1.0f;  // 배고플 때 초당 피해량
    [SerializeField, Range(0, 1f)] private float regenThreshold = 0.9f; // 회복 시작 허기 비율

    public void Init(Character _owner)
    {
        owner = _owner;
    }

    private void Update()
    {
        if (owner == null) return;

        // 허기 감소
        owner.Hunger.CurrentValue -= hungerDecayRate * Time.deltaTime;

        // 체력 자동 회복
        if (owner.Hunger.CurrentValue >= owner.Hunger.Value * regenThreshold)
        {
            owner.Health.CurrentValue += healthRegenRate * Time.deltaTime;
        }

        // 아사
        if (owner.Hunger.CurrentValue <= 0)
        {
            owner.Health.CurrentValue -= starvationDamageRate * Time.deltaTime;
            if (owner.Health.CurrentValue <= 0) Die();
        }
    }

    public void TakeDamage(float damage)
    {
        if (owner == null) return;

        float finalDamage = Mathf.Max(0, damage - owner.Defense.Value);
        owner.Health.CurrentValue -= finalDamage;

        Debug.Log($"[Player] 피격됨. 남은체력: {owner.Health.CurrentValue}");

        if (owner.Health.CurrentValue <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
    }

    public void ConsumeHunger(float amount) => owner.Hunger.CurrentValue -= amount;
    public void Heal(float amount) => owner.Health.CurrentValue += amount;
}