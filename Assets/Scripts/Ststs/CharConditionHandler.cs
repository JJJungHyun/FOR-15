using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharConditionHandler : MonoBehaviour, IDamageable
{
    private Character owner;
    private Rigidbody2D rb;

    [Header("Hunger Settings")]
    [SerializeField] private float hungerDecayRate = 0.5f;
    [SerializeField] private float healthRegenRate = 0.2f;
    [SerializeField] private float starvationDamageRate = 1.0f;
    [SerializeField, Range(0, 1f)] private float regenThreshold = 0.9f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackDuration = 0.1f;

    private readonly List<ConditionEffect> activeEffects = new List<ConditionEffect>();
    private readonly List<ConditionEffect> effectsToRemove = new List<ConditionEffect>();

    public void Init(Character _owner)
    {
        owner = _owner;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (owner == null || owner.IsDead) return;

        HandleBasicConditions();
        HandleStatusEffects();
        owner.CheckDeath();
    }

    private void HandleBasicConditions()
    {
        owner.Hunger.CurrentValue -= hungerDecayRate * Time.deltaTime;

        if (owner.Hunger.CurrentValue >= owner.Hunger.Value * regenThreshold)
        {
            owner.Heal(healthRegenRate * Time.deltaTime);
        }

        if (owner.Hunger.CurrentValue <= 0f)
        {
            owner.TakeDirectDamage(starvationDamageRate * Time.deltaTime);
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

            if (owner.IsDead) break;
        }

        foreach (var effect in effectsToRemove)
        {
            effect.OnEnd();
            activeEffects.Remove(effect);
        }
    }

    public void ApplyEffect(ConditionEffect newEffect)
    {
        if (owner == null || owner.IsDead) return;

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
        if (owner == null || owner.IsDead) return;
        owner.TakeDamage(damage, attackerPosition);
    }

    public void ApplyKnockback(Vector2 attackerPos)
    {
        if (rb == null || owner == null || owner.IsDead) return;

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

    public void ConsumeHunger(float amount)
    {
        if (owner == null || owner.IsDead) return;
        owner.Hunger.CurrentValue -= amount;
        owner.CheckDeath();
    }

    public void Heal(float amount)
    {
        if (owner == null) return;
        owner.Heal(amount);
    }
}