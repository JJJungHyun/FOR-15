using System;
using UnityEngine;
using CharacterStats;

[Serializable]
public class ItemSlot
{
    public Item Item;
    public int Amount;

    // 슬롯의 데이터가 변했을 때 UI에 알리기 위한 이벤트
    public event Action<ItemSlot> OnSlotChanged;

    public void UpdateSlot()
    {
        OnSlotChanged?.Invoke(this);
    }
}