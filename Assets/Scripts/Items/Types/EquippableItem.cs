using UnityEngine;
using CharacterStats;
public enum EquipmentType
{
    Helmet, Chestplate, Gloves, Boots, Weapon, Accessory,
}

public enum ToolType { None = 0, Slingshot = 1, Axe = 2, Pickaxe = 3 }

[CreateAssetMenu(menuName = "Items/Equippable Item")]
public class EquippableItem : Item
{
    [Header("Weapon Ability Configuration")]
    [SerializeField] private ScriptableObject weaponAbilityAsset;

    public IWeaponAbility WeaponAbility => weaponAbilityAsset as IWeaponAbility;

    [Header("Animation Settings")]
    public ToolType ToolType;

    public int StrengthBonus;
    public int DefenseBonus;
    [Space]
    public float StrengthPercentBonus;
    public float DefensePercentBonus;
    [Space]
    public EquipmentType EquipmentType;

    public void Equip(Character c)
    {
        if (StrengthBonus != 0) c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));
        if (DefenseBonus != 0) c.Defense.AddModifier(new StatModifier(DefenseBonus, StatModType.Flat, this));
        if (StrengthPercentBonus != 0) c.Strength.AddModifier(new StatModifier(StrengthPercentBonus, StatModType.PercentMult, this));
        if (DefenseBonus != 0) c.Defense.AddModifier(new StatModifier(DefensePercentBonus, StatModType.PercentMult, this));
    }

    public void Unequip(Character c)
    {
        c.Strength.RemoveAllModifiersFromSource(this);
        c.Defense.RemoveAllModifiersFromSource(this);
    }

    public override string GetItemType() => EquipmentType.ToString();
    public override string GetDescription()
    {
        sb.Length = 0;
        AddStatText(StrengthBonus, "힘");
        AddStatText(DefenseBonus, "방어");
        AddStatText(StrengthPercentBonus, "힘", isPercent: true);
        AddStatText(DefensePercentBonus, "방어", isPercent: true);
        return sb.ToString();
    }

    private void AddStatText(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
            if (sb.Length > 0) sb.AppendLine();
            if (value > 0) sb.Append("+");
            if (isPercent) { sb.Append(value * 100); sb.Append("% "); }
            else { sb.Append(value); sb.Append(" "); }
            sb.Append(statName);
        }
    }
}