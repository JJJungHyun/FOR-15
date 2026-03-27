using UnityEngine;
using CharacterStats;

[CreateAssetMenu(menuName = "Items/Equippable Item")]
public class EquippableItem : Item
{
    public int StrengthBonus;
    public int DefenseBonus;
    [Space]
    public float StrengthPercentBonus;
    public float DefensePercentBonus;
    [Space]
    public EquipmentType EquipmentType;

    public void Equip(Character c)
    {
        if (StrengthBonus != 0)
            c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));

        if (DefenseBonus != 0)
            c.Defense.AddModifier(new StatModifier(DefenseBonus, StatModType.Flat, this));

        if (StrengthPercentBonus != 0)
            c.Strength.AddModifier(new StatModifier(StrengthPercentBonus, StatModType.PercentMult, this));

        if (DefensePercentBonus != 0)
            c.Defense.AddModifier(new StatModifier(DefensePercentBonus, StatModType.PercentMult, this));
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
        AddStatText(StrengthBonus, "Èû");
        AddStatText(DefenseBonus, "¹æ¾î");
        AddStatText(StrengthPercentBonus, "Èû", isPercent: true);
        AddStatText(DefensePercentBonus, "¹æ¾î", isPercent: true);
        return sb.ToString();
    }

    private void AddStatText(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
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
}


public enum EquipmentType
{
    Helmet, Chestplate, Gloves, Boots, Weapon, Accessory,
}