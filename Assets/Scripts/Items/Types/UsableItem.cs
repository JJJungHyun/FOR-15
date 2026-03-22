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

        foreach (var effect in Effects)
        {
            if (effect != null)
            {
                effect.ExecuteEffect(this, c);
            }
        }
    }

    public override string GetItemType() => "소모품";

    public override string GetDescription()
    {
        sb.Length = 0;

        if (Effects == null || Effects.Count == 0)
        {
            return "효과 없음";
        }

        for (int i = 0; i < Effects.Count; i++)
        {
            if (Effects[i] == null) continue;
             
            sb.Append(Effects[i].GetDescription());

            if (i < Effects.Count - 1) sb.AppendLine();
        }

        return sb.ToString();
    }
}