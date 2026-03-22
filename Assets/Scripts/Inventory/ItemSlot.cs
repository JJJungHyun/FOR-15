using System;
using UnityEngine;
using CharacterStats;

[Serializable]
public class ItemSlot
{
    public Item Item;
    public int Amount;

    public event Action<ItemSlot> OnSlotChanged;

    public void UpdateSlot()
    {
        OnSlotChanged?.Invoke(this);
    }
}