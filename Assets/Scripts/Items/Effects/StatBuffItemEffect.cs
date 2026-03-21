using System.Collections;
using UnityEngine;
using CharacterStats;

[CreateAssetMenu(menuName = "Items/Effects/Stat Buff")]
public class StatBuffItemEffect : UsableItemEffect
{
    public float Value;
    public StatModType ModType;
    public float Duration;

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        // Modifier 생성 (Source를 parentItem으로 설정하여 추적 가능하게 함)
        StatModifier mod = new StatModifier(Value, ModType, parentItem);
        character.Strength.AddModifier(mod);

        // 일정 시간 후 제거 (Character의 코루틴 활용)
        character.StartCoroutine(RemoveBuffAfterDelay(character, mod, Duration));
    }

    private IEnumerator RemoveBuffAfterDelay(Character character, StatModifier mod, float delay)
    {
        yield return new WaitForSeconds(delay);
        character.Strength.RemoveModifier(mod);
    }

    public override string GetDescription() => $"{Duration}초 동안 공격력 {Value}{(ModType == StatModType.Flat ? "" : "%")} 증가";
}