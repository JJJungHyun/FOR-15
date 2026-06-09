using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Character character;
    [SerializeField] private Inventory inventory;

    [Header("Equipment Slots Mapping")]
    [SerializeField] private List<EquipmentSlotUI> equipmentSlots = new List<EquipmentSlotUI>();

    private void Awake()
    {
        InitializeStartingEquipments();
    }

    private void OnEnable()
    {
        BaseItemSlotUI.OnSlotRightClickEvent += HandleEquipmentRightClick;
        BaseItemSlotUI.OnSlotDropEvent += HandleEquipmentDrop;
    }

    private void OnDisable()
    {
        BaseItemSlotUI.OnSlotRightClickEvent -= HandleEquipmentRightClick;
        BaseItemSlotUI.OnSlotDropEvent -= HandleEquipmentDrop;
    }

    private void InitializeStartingEquipments()
    {
        if (equipmentSlots == null) return;
        foreach (var slotUI in equipmentSlots)
        {
            if (slotUI != null && slotUI.Slot != null && slotUI.Slot.Item is EquippableItem equippable)
            {
                equippable.Equip(character);
            }
        }
    }

    private void HandleEquipmentDrop(BaseItemSlotUI dropSlotUI)
    {
    }

    private void HandleEquipmentRightClick(BaseItemSlotUI slotUI)
    {
        Item item = slotUI.Slot.Item;
        if (item == null) return;

        if (slotUI is EquipmentSlotUI equipSlot)
        {
            UnequipViaRightClick(equipSlot);
            return;
        }

        if (item is EquippableItem equippable)
        {
            EquipViaRightClick(slotUI, equippable);
        }
    }

    private void EquipViaRightClick(BaseItemSlotUI sourceSlot, EquippableItem item)
    {
        if (equipmentSlots == null) return;

        foreach (var targetSlot in equipmentSlots)
        {
            if (targetSlot != null && targetSlot.SlotType == item.EquipmentType)
            {
                if (targetSlot.Slot.Item is EquippableItem oldEquip) oldEquip.Unequip(character);
                item.Equip(character);

                Item temp = targetSlot.Slot.Item;
                targetSlot.Slot.Item = item;
                sourceSlot.Slot.Item = temp;

                targetSlot.Slot.UpdateSlot();
                sourceSlot.Slot.UpdateSlot();
                return;
            }
        }
    }

    private void UnequipViaRightClick(EquipmentSlotUI sourceSlot)
    {
        if (inventory == null) return;

        EquippableItem item = sourceSlot.Slot.Item as EquippableItem;
        if (item == null) return;

        if (inventory.AddItemCustomPriority(item, 1))
        {
            item.Unequip(character);
            sourceSlot.Slot.Item = null;
            sourceSlot.Slot.Amount = 0;
            sourceSlot.Slot.UpdateSlot();
        }
    }
}