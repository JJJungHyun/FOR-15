using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour, IItemContainer
{
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    public event Action OnItemsChanged;
    public event Action<Item> OnItemAdded;
    public virtual bool AddItem(Item item)
    {
        if (item == null)
        {
            return false;
        }

        // 기존 스택에 추가
        foreach (var slot in itemSlots)
        {
            if (slot.Item != null && slot.Item.ID == item.ID && slot.Amount < item.MaximumStacks)
            {
                slot.Amount++;
                slot.UpdateSlot();
                OnItemsChanged?.Invoke();
                OnItemAdded?.Invoke(slot.Item);
                return true;
            }
        }
        // 빈 슬롯에 추가
        foreach (var slot in itemSlots)
        {
            if (slot.Item == null)
            {
                slot.Item = item.GetCopy();
                slot.Amount = 1;
                slot.UpdateSlot();
                OnItemsChanged?.Invoke();
                OnItemAdded?.Invoke(slot.Item);
                return true;
            }
        }
        return false;
    }

    public virtual bool RemoveItemByID(string itemID)
    {
        foreach (var slot in itemSlots)
        {
            // ID가 일치하는지 확인
            if (slot.Item != null && slot.Item.ID == itemID)
            {
                slot.Amount--;
                if (slot.Amount <= 0)
                {
                    slot.Item.Destroy();
                    slot.Item = null;
                }
                slot.UpdateSlot();
                OnItemsChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public virtual bool RemoveItem(Item item) => RemoveItemByID(item.ID);

    public virtual bool IsFull() => itemSlots.Find(slot => slot.Item == null) == null;

    public virtual int ItemCount(string itemID)
    {
        int count = 0;
        foreach (var slot in itemSlots)
            if (slot.Item != null && slot.Item.ID == itemID) count += slot.Amount;
        return count;
    }

    public virtual void Clear()
    {
        foreach (var slot in itemSlots)
        {
            if (slot.Item != null) slot.Item.Destroy();
            slot.Item = null;
            slot.Amount = 0;
            slot.UpdateSlot();
        }
    }
}