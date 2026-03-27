using UnityEngine;
using CharacterStats;

public enum RestoreStatType
{
    Health,
    Hunger,
}

[CreateAssetMenu(menuName = "Items/Effects/Restore Stat")]
public class RestoreStatItemEffect : UsableItemEffect
{
    public RestoreStatType TargetStat; // 회복할 대상 스탯
    public float RestoreAmount;        // 회복량
    public bool IsPercent;             // 퍼센트 회복 여부

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        ClampedStat target = GetTargetStat(character);
        if (target == null) return;

        float finalAmount = 0;

        if (IsPercent)
        {
            finalAmount = target.Value * (RestoreAmount / 100f);
        }
        else
        {
            finalAmount = RestoreAmount;
        }

        target.CurrentValue += finalAmount;
    }

    private ClampedStat GetTargetStat(Character character)
    {
        return TargetStat switch
        {
            RestoreStatType.Health => character.Health,
            RestoreStatType.Hunger => character.Hunger,
            _ => null
        };
    }

    public override string GetDescription()
    {
        string statName = TargetStat == RestoreStatType.Health ? "체력" : "허기";
        return $"{statName} {RestoreAmount}{(IsPercent ? "%" : "")} 회복";
    }
}