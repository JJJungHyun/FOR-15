using UnityEngine;
using CharacterStats;

public class CharConditionHandler : MonoBehaviour
{
    private Character owner;

    public void Init(Character _owner)
    {
        owner = _owner;
    }

    private void Update()
    {
        if (owner == null) return;

        owner.Hunger.CurrentValue -= 0.5f * Time.deltaTime;

        if (owner.Hunger.CurrentValue >= owner.Hunger.Value * 0.9f)
        {
            owner.Health.CurrentValue += 0.2f * Time.deltaTime;
        }

        if (owner.Hunger.CurrentValue <= 0)
        {
            owner.Health.CurrentValue -= 1.0f * Time.deltaTime;
        }
    }
    public void ConsumeHunger(float amount)
    {
        owner.Hunger.CurrentValue -= amount;
    }

    public void Heal(float amount)
    {
        owner.Health.CurrentValue += amount;
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(0, damage - owner.Defense.Value);
        owner.Health.CurrentValue -= finalDamage;

        if (owner.Health.CurrentValue <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Ä³¸¯ÅÍ »ç¸Á");
    }
}
