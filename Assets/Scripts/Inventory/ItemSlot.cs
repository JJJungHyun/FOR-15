using System;

[Serializable]
public class ItemSlot
{
    public Item Item;
    public int Amount;

    public event Action<ItemSlot, Item> OnSlotChangedWithPrev;
    public event Action<ItemSlot> OnSlotChanged;

    private EquippableItem trackedDurabilityItem;

    public void UpdateSlot(Item previousItem = null)
    {
        if (TrackDurabilityItem()) return;

        OnSlotChanged?.Invoke(this);
        OnSlotChangedWithPrev?.Invoke(this, previousItem);
    }

    private bool TrackDurabilityItem()
    {
        EquippableItem itemToTrack = Item as EquippableItem;
        if (itemToTrack != null && !itemToTrack.HasDurability)
        {
            itemToTrack = null;
        }

        if (trackedDurabilityItem == itemToTrack)
        {
            if (trackedDurabilityItem != null && trackedDurabilityItem.IsBroken)
            {
                HandleTrackedItemBroken(trackedDurabilityItem);
                return true;
            }
            return false;
        }

        if (trackedDurabilityItem != null)
        {
            trackedDurabilityItem.OnBroken -= HandleTrackedItemBroken;
        }

        trackedDurabilityItem = itemToTrack;

        if (trackedDurabilityItem != null)
        {
            trackedDurabilityItem.OnBroken += HandleTrackedItemBroken;

            if (trackedDurabilityItem.IsBroken)
            {
                HandleTrackedItemBroken(trackedDurabilityItem);
                return true;
            }
        }

        return false;
    }

    private void HandleTrackedItemBroken(EquippableItem brokenItem)
    {
        if (!ReferenceEquals(Item, brokenItem)) return;

        Item previousItem = Item;
        Item = null;
        Amount = 0;

        previousItem?.Destroy();
        UpdateSlot(previousItem);
    }
}