using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Usable Item")]
public class UsableItem : Item
{
    public static event Action<UsableItem, Character> OnUsed;

    [Header("Usage Settings")]
    [Tooltip("Whether this item is consumed after use.")]
    public bool IsConsumable = true;

    [Tooltip("Effects executed when this item is used.")]
    public List<UsableItemEffect> Effects;

    public override void Use(Character c)
    {
        if (Effects == null || Effects.Count == 0)
        {
            Debug.LogWarning($"{ItemName}: no usable effects are assigned.");
            return;
        }

        foreach (UsableItemEffect effect in Effects)
        {
            if (effect != null)
            {
                effect.ExecuteEffect(this, c);
            }
        }

        OnUsed?.Invoke(this, c);
    }

    public override string GetItemType() => "Usable";

    public override string GetDescription()
    {
        sb.Length = 0;

        if (Effects == null || Effects.Count == 0)
        {
            return "No effect";
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