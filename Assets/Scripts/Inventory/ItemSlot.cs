using System;

[Serializable]
public class ItemSlot
{
    public Item Item;
    public int Amount;

    public event Action<ItemSlot, Item> OnSlotChangedWithPrev;
    public event Action<ItemSlot> OnSlotChanged;

    public void UpdateSlot(Item previousItem = null)
    {
        OnSlotChanged?.Invoke(this);
        OnSlotChangedWithPrev?.Invoke(this, previousItem);
    }
}