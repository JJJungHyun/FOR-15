using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharConditionHandler : MonoBehaviour, IDamageable
{
    private Character owner;
    private Rigidbody2D rb;

    [Header("Hunger Settings")]
    [SerializeField] private float hungerDecayRate = 0.5f;       // 초당 허기 감소량
    [SerializeField] private float healthRegenRate = 0.2f;      // 배부를 때 초당 회복량
    [SerializeField] private float starvationDamageRate = 1.0f;  // 배고플 때 초당 피해량
    [SerializeField, Range(0, 1f)] private float regenThreshold = 0.9f; // 회복 시작 허기 비율

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 3f;      // 넉백 강도
    [SerializeField] private float knockbackDuration = 0.1f; // 넉백 지속 시간


    private List<ConditionEffect> activeEffects = new List<ConditionEffect>();
    private List<ConditionEffect> effectsToRemove = new List<ConditionEffect>();

    public void Init(Character _owner)
    {
        owner = _owner;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (owner == null) return;

        HandleBasicConditions();
        HandleStatusEffects();
    }

    private void HandleBasicConditions()
    {
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

    private void HandleStatusEffects()
    {
        if (activeEffects.Count == 0) return;

        effectsToRemove.Clear();

        for (int i = 0; i < activeEffects.Count; i++)
        {
            activeEffects[i].Tick(Time.deltaTime);
            activeEffects[i].OnUpdate();

            if (activeEffects[i].Duration <= 0)
            {
                effectsToRemove.Add(activeEffects[i]);
            }
        }

        foreach (var effect in effectsToRemove)
        {
            effect.OnEnd();
            activeEffects.Remove(effect);
        }
    }

    public void ApplyEffect(ConditionEffect newEffect)
    {
        ConditionEffect existing = activeEffects.Find(e => e.Name == newEffect.Name);

        if (existing != null)
        {
            if (newEffect.Power >= existing.Power)
            {
                existing.ResetDuration(newEffect.Duration); 
                existing.SetPower(newEffect.Power);   

            }
            return;
        }

        activeEffects.Add(newEffect);
        newEffect.OnStart();
    }

    public void TakeDamage(float damage, Vector2 attackerPosition)
    {
        if (owner == null) return;

        float finalDamage = Mathf.Max(0, damage - owner.Defense.Value);
        owner.Health.CurrentValue -= finalDamage;

        ApplyKnockback(attackerPosition);

        Debug.Log($"[Player] 피격됨. 남은체력: {owner.Health.CurrentValue}");
        if (owner.Health.CurrentValue <= 0) Die();
    }

    private void ApplyKnockback(Vector2 attackerPos)
    {
        if (rb == null) return;

        Vector2 knockbackDir = ((Vector2)transform.position - attackerPos).normalized;
        StartCoroutine(KnockbackRoutine(knockbackDir));
    }

    private IEnumerator KnockbackRoutine(Vector2 dir)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector2.zero;
    }

    public List<ConditionEffect> GetActiveEffects()
    {
        return activeEffects;
    }

    private void Die()
    {
        Debug.Log("플레이어 사망");
    }

    public void ConsumeHunger(float amount) => owner.Hunger.CurrentValue -= amount;
    public void Heal(float amount) => owner.Health.CurrentValue += amount;
}