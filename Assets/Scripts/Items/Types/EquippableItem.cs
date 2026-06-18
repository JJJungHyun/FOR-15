using System;
using CharacterStats;
using UnityEngine;

public enum EquipmentType
{
    Helmet, Chestplate, Gloves, Boots, Weapon, Accessory,
}

public enum ToolType { None = 0, Slingshot = 1, Axe = 2 }

[CreateAssetMenu(menuName = "Items/Equippable Item")]
public class EquippableItem : Item
{
    [Header("Weapon Ability Configuration")]
    [SerializeField] private ScriptableObject weaponAbilityAsset;

    [Header("Animation Settings")]
    public ToolType ToolType;

    [Header("Durability Settings")]
    [SerializeField] private bool hasDurability = true;
    [SerializeField] private int maxDurability = 100;

    public event Action OnDurabilityChanged;
    public event Action<EquippableItem> OnBroken;

    public IWeaponAbility WeaponAbility => weaponAbilityAsset as IWeaponAbility;
    public int MaxDurability => Mathf.Max(1, maxDurability);
    public int CurrentDurability { get; set; }
    public bool HasDurability => hasDurability;
    public bool IsBroken => hasDurability && CurrentDurability <= 0;

    public int StrengthBonus;
    public int DefenseBonus;
    [Space]
    public float StrengthPercentBonus;
    public float DefensePercentBonus;
    [Space]
    public EquipmentType EquipmentType;

    private bool brokenNotified;

    public override Item GetCopy()
    {
        EquippableItem clone = Instantiate(this);
        clone.CurrentDurability = clone.MaxDurability;
        clone.brokenNotified = false;
        return clone;
    }

    public void Equip(Character c)
    {
        if (c == null) return;

        if (StrengthBonus != 0) c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));
        if (DefenseBonus != 0) c.Defense.AddModifier(new StatModifier(DefenseBonus, StatModType.Flat, this));
        if (StrengthPercentBonus != 0) c.Strength.AddModifier(new StatModifier(StrengthPercentBonus, StatModType.PercentMult, this));
        if (DefensePercentBonus != 0) c.Defense.AddModifier(new StatModifier(DefensePercentBonus, StatModType.PercentMult, this));
    }

    public void Unequip(Character c)
    {
        if (c == null) return;

        c.Strength.RemoveAllModifiersFromSource(this);
        c.Defense.RemoveAllModifiersFromSource(this);
    }

    public void ConsumeDurability(int amount, Character owner)
    {
        if (!hasDurability || amount <= 0 || brokenNotified) return;

        CurrentDurability = Mathf.Max(0, CurrentDurability - amount);
        OnDurabilityChanged?.Invoke();

        Debug.Log($"{ItemName} durability -{amount}. Current: {CurrentDurability}/{MaxDurability}");

        if (CurrentDurability <= 0)
        {
            HandleBroken(owner);
        }
    }

    private void HandleBroken(Character owner)
    {
        if (brokenNotified) return;

        brokenNotified = true;
        CurrentDurability = 0;

        Unequip(owner);
        Debug.LogWarning($"{ItemName} broke and was removed.");
        OnBroken?.Invoke(this);
    }

    public override string GetItemType() => EquipmentType.ToString();

    public override string GetDescription()
    {
        sb.Length = 0;
        AddStatText(StrengthBonus, "Strength");
        AddStatText(DefenseBonus, "Defense");
        AddStatText(StrengthPercentBonus, "Strength", true);
        AddStatText(DefensePercentBonus, "Defense", true);
        return sb.ToString();
    }

    private void AddStatText(float value, string statName, bool isPercent = false)
    {
        if (value == 0) return;

        if (sb.Length > 0) sb.AppendLine();
        if (value > 0) sb.Append("+");

        if (isPercent)
        {
            sb.Append(value * 100);
            sb.Append("% ");
        }
        else
        {
            sb.Append(value);
            sb.Append(" ");
        }

        sb.Append(statName);
    }
}