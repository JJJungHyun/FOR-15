using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Character character;
    [SerializeField] private Image dragIcon;

    [Header("Equipment Slots")]
    [SerializeField] private List<EquipmentSlotUI> equipmentSlots = new List<EquipmentSlotUI>();

    private BaseItemSlotUI draggedSlot;

    private void OnEnable()
    {
        BaseItemSlotUI.OnSlotRightClickEvent += HandleRightClick;
        BaseItemSlotUI.OnSlotBeginDragEvent += HandleBeginDrag;
        BaseItemSlotUI.OnSlotEndDragEvent += HandleEndDrag;
        BaseItemSlotUI.OnSlotDragEvent += HandleDrag;
        BaseItemSlotUI.OnSlotDropEvent += HandleDrop;
    }

    private void OnDisable()
    {
        BaseItemSlotUI.OnSlotRightClickEvent -= HandleRightClick;
        BaseItemSlotUI.OnSlotBeginDragEvent -= HandleBeginDrag;
        BaseItemSlotUI.OnSlotEndDragEvent -= HandleEndDrag;
        BaseItemSlotUI.OnSlotDragEvent -= HandleDrag;
        BaseItemSlotUI.OnSlotDropEvent -= HandleDrop;

        if (draggedSlot != null) HandleEndDrag(draggedSlot);
    }

    #region Drag & Drop 
    private void HandleBeginDrag(BaseItemSlotUI slotUI)
    {
        if (slotUI.Slot.Item == null) return;

        draggedSlot = slotUI;
        draggedSlot.SetAlpha(0.5f);
        dragIcon.sprite = slotUI.Slot.Item.Icon;
        dragIcon.SetNativeSize();
        dragIcon.gameObject.SetActive(true);
        UpdateDragIconPosition();
    }

    private void HandleDrag(BaseItemSlotUI slotUI) => UpdateDragIconPosition();

    private void UpdateDragIconPosition()
    {
        if (draggedSlot != null) dragIcon.transform.position = Input.mousePosition;
    }

    private void HandleEndDrag(BaseItemSlotUI slotUI)
    {
        if (draggedSlot != null) draggedSlot.SetSlot(draggedSlot.Slot);

        draggedSlot = null;
        dragIcon.gameObject.SetActive(false);
    }

    private void HandleDrop(BaseItemSlotUI dropSlotUI)
    {
        if (draggedSlot == null || dropSlotUI == null) return;

        if (dropSlotUI.CanReceiveItem(draggedSlot.Slot.Item) && draggedSlot.CanReceiveItem(dropSlotUI.Slot.Item))
        {
            HandleEquipmentLogic(draggedSlot, dropSlotUI);

            ItemSlot source = draggedSlot.Slot;
            ItemSlot target = dropSlotUI.Slot;

            Item tempItem = source.Item;
            int tempAmount = source.Amount;

            source.Item = target.Item;
            source.Amount = target.Amount;

            target.Item = tempItem;
            target.Amount = tempAmount;

            source.UpdateSlot();
            target.UpdateSlot();

            RefreshTooltip(dropSlotUI);
        }
    }

    private void HandleEquipmentLogic(BaseItemSlotUI sourceSlot, BaseItemSlotUI targetSlot)
    {
        if (!(sourceSlot is EquipmentSlotUI) && targetSlot is EquipmentSlotUI)
        {
            if (targetSlot.Slot.Item is EquippableItem oldItem) oldItem.Unequip(character);
            if (sourceSlot.Slot.Item is EquippableItem newItem) newItem.Equip(character);
        }
        else if (sourceSlot is EquipmentSlotUI && !(targetSlot is EquipmentSlotUI))
        {
            if (sourceSlot.Slot.Item is EquippableItem unequipItem) unequipItem.Unequip(character);
        }
        else if (sourceSlot is EquipmentSlotUI && targetSlot is EquipmentSlotUI)
        {
            if (sourceSlot.Slot.Item is EquippableItem item1) item1.Unequip(character);
            if (targetSlot.Slot.Item is EquippableItem item2) item2.Equip(character);
        }
    }
    #endregion

    #region Right Click 
    private void HandleRightClick(BaseItemSlotUI slotUI)
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
        else if (item is UsableItem usable)
        {
            usable.Use(character);
            if (usable.IsConsumable)
            {
                slotUI.Slot.Amount--;
                if (slotUI.Slot.Amount <= 0) slotUI.Slot.Item = null;
                slotUI.Slot.UpdateSlot();
                RefreshTooltip(slotUI);
            }
        }
    }

    private void RefreshTooltip(BaseItemSlotUI slotUI)
    {
        if (slotUI != null && slotUI.Slot.Item != null)
            ItemTooltip.Instance?.ShowTooltip(slotUI.Slot.Item);
        else
            ItemTooltip.Instance?.HideTooltip();
    }

    private void EquipViaRightClick(BaseItemSlotUI sourceSlot, EquippableItem item)
    {
        foreach (var targetSlot in equipmentSlots)
        {
            if (targetSlot.SlotType == item.EquipmentType)
            {
                if (targetSlot.Slot.Item is EquippableItem oldEquip) oldEquip.Unequip(character);
                item.Equip(character);

                Item temp = targetSlot.Slot.Item;
                targetSlot.Slot.Item = item;
                sourceSlot.Slot.Item = temp;

                targetSlot.Slot.UpdateSlot();
                sourceSlot.Slot.UpdateSlot();
                RefreshTooltip(sourceSlot);
                return;
            }
        }
    }

    private void UnequipViaRightClick(EquipmentSlotUI sourceSlot)
    {
        EquippableItem item = sourceSlot.Slot.Item as EquippableItem;
        if (item == null) return;

        if (inventory.AddItemCustomPriority(item, 1))
        {
            item.Unequip(character);
            sourceSlot.Slot.Item = null;
            sourceSlot.Slot.Amount = 0;
            sourceSlot.Slot.UpdateSlot();
            RefreshTooltip(sourceSlot);
        }
    }
    #endregion
}