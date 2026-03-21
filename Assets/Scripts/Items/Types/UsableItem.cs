using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Usable Item")]
public class UsableItem : Item
{
    [Header("Usage Settings")]
    public bool IsConsumable = true; // 소모품 여부

    [Tooltip("이 아이템을 사용했을 때 발생할 효과 리스트 (ScriptableObject)")]
    public List<UsableItemEffect> Effects;

    public override void Use(Character c)
    {
        if (Effects == null || Effects.Count == 0)
        {
            Debug.LogWarning($"{ItemName}: 실행할 효과가 없습니다.");
            return;
        }

        // 1. 모든 효과 실행
        foreach (var effect in Effects)
        {
            if (effect != null)
            {
                effect.ExecuteEffect(this, c);
            }
        }

        // 2. 소모 처리 로직
        // InventoryController에서 수량을 줄이는 방식이 일반적이지만, 
        // 아이템 자체에서도 소모 여부를 반환하거나 처리할 준비가 되어있어야 합니다.
    }

    public override string GetItemType() => "소모품";

    public override string GetDescription()
    {
        // 부모(Item)의 StringBuilder 사용
        sb.Length = 0;

        if (Effects == null || Effects.Count == 0)
        {
            return "효과 없음";
        }

        for (int i = 0; i < Effects.Count; i++)
        {
            if (Effects[i] == null) continue;

            sb.Append(Effects[i].GetDescription());

            // 마지막 줄이 아니면 줄바꿈 추가
            if (i < Effects.Count - 1) sb.AppendLine();
        }

        return sb.ToString();
    }
}