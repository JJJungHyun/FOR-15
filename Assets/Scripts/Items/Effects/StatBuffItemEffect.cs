using System.Collections;
using UnityEngine;
using CharacterStats;

[CreateAssetMenu(menuName = "Items/Effects/Stat Buff")]
public class StatBuffItemEffect : UsableItemEffect
{
    public enum TargetStatType { Strength, Defense }

    public TargetStatType TargetStat;
    public float Value;
    public StatModType ModType;
    public float Duration;

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        Stat target = (TargetStat == TargetStatType.Strength) ? character.Strength : character.Defense;

        StatModifier mod = new StatModifier(Value, ModType, parentItem);
        target.AddModifier(mod);

        character.StartCoroutine(RemoveBuffAfterDelay(target, mod, Duration));
    }

    private IEnumerator RemoveBuffAfterDelay(Stat stat, StatModifier mod, float delay)
    {
        yield return new WaitForSeconds(delay);
        stat.RemoveModifier(mod); 
    }

    public override string GetDescription()
        => $"{Duration}초 동안 {TargetStat} {Value}{(ModType == StatModType.Flat ? "" : "%")} 증가";
}