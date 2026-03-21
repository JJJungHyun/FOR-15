using UnityEngine;
using CharacterStats;

[CreateAssetMenu(menuName = "Items/Effects/Heal")]
public class HealItemEffect : UsableItemEffect
{
    public float Amount;
    public bool IsPercent; // 최대 체력 대비 % 회복 여부

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if (IsPercent)
        {
            // 최대값(Value)의 일정 비율만큼 현재값(CurrentValue) 증가
            character.Health.CurrentValue += character.Health.Value * (Amount / 100f);
        }
        else
        {
            character.Health.CurrentValue += Amount;
        }
        Debug.Log($"{parentItem.ItemName} 사용: 체력 {Amount}{(IsPercent ? "%" : "")} 회복");
    }

    public override string GetDescription() => $"체력 {Amount}{(IsPercent ? "%" : "")} 회복";
}