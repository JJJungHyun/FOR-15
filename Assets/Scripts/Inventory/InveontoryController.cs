using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private Character character;
    [SerializeField] private Image dragIcon;

    [Header("Tooltips")]
    [SerializeField] private ItemTooltip itemTooltip;
    [SerializeField] private StatTooltip statTooltip;

    private BaseItemSlotUI draggedSlot;

    private void Awake()
    {
        if (itemTooltip != null) itemTooltip.gameObject.SetActive(true);
        if (statTooltip != null) statTooltip.gameObject.SetActive(true);
        if (dragIcon != null) dragIcon.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (itemTooltip != null) itemTooltip.gameObject.SetActive(false);
        if (statTooltip != null) statTooltip.gameObject.SetActive(false);

        RefreshAllSlots();
    }

    public Inventory GetInventory() => inventory;

    public void RefreshAllSlots()
    {
        var allSlots = GetComponentsInChildren<BaseItemSlotUI>(true);
        foreach (var slotUI in allSlots)
        {
            if (slotUI.Slot != null)
            {
                slotUI.SetSlot(slotUI.Slot);
            }
        }
    }

    private void OnEnable()
    {
        // ❌ PlayerInputHandler.OnToggleCombinedUI 구독 제거 (CharPanelMgr가 전담함)

        BaseItemSlotUI.OnSlotRightClickEvent += HandleRightClick;
        BaseItemSlotUI.OnSlotBeginDragEvent += HandleBeginDrag;
        BaseItemSlotUI.OnSlotEndDragEvent += HandleEndDrag;
        BaseItemSlotUI.OnSlotDragEvent += HandleDrag;
        BaseItemSlotUI.OnSlotDropEvent += HandleDrop;
        PlayerInputHandler.OnQuickSlotPressed += HandleQuickSlotPressed;
    }

    private void OnDisable()
    {
        // ❌ PlayerInputHandler.OnToggleCombinedUI 구독 해제 제거

        BaseItemSlotUI.OnSlotRightClickEvent -= HandleRightClick;
        BaseItemSlotUI.OnSlotBeginDragEvent -= HandleBeginDrag;
        BaseItemSlotUI.OnSlotEndDragEvent -= HandleEndDrag;
        BaseItemSlotUI.OnSlotDragEvent -= HandleDrag;
        BaseItemSlotUI.OnSlotDropEvent -= HandleDrop;
        PlayerInputHandler.OnQuickSlotPressed -= HandleQuickSlotPressed;

        if (draggedSlot != null) HandleEndDrag(draggedSlot);
    }

    #region Drag & Drop 
    private void HandleBeginDrag(BaseItemSlotUI slotUI)
    {
        if (slotUI.Slot == null || slotUI.Slot.Item == null) return;

        draggedSlot = slotUI;
        draggedSlot.SetAlpha(0.5f);

        if (dragIcon != null)
        {
            dragIcon.sprite = slotUI.Slot.Item.Icon;
            dragIcon.SetNativeSize();
            dragIcon.gameObject.SetActive(true);
            UpdateDragIconPosition();
        }
    }

    private void HandleDrag(BaseItemSlotUI slotUI) => UpdateDragIconPosition();

    private void UpdateDragIconPosition()
    {
        if (draggedSlot != null && dragIcon != null)
            dragIcon.transform.position = Input.mousePosition;
    }

    private void HandleEndDrag(BaseItemSlotUI slotUI)
    {
        if (dragIcon != null) dragIcon.gameObject.SetActive(false);

        if (draggedSlot != null)
        {
            if (draggedSlot.Slot != null)
            {
                draggedSlot.Slot.UpdateSlot();
            }
            draggedSlot = null;
        }
    }
    private void HandleDrop(BaseItemSlotUI dropSlotUI)
    {
        if (draggedSlot == null || dropSlotUI == null) return;
        if (draggedSlot.Slot == null || dropSlotUI.Slot == null) return;

        if (dropSlotUI.CanReceiveItem(draggedSlot.Slot.Item) && draggedSlot.CanReceiveItem(dropSlotUI.Slot.Item))
        {
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
    #endregion

    #region Right Click 
    private void HandleRightClick(BaseItemSlotUI slotUI)
    {
        if (slotUI.Slot == null) return;

        UseSlotItem(slotUI.Slot);
        RefreshTooltip(slotUI);
    }

    private void RefreshTooltip(BaseItemSlotUI slotUI)
    {
        if (slotUI != null && slotUI.Slot != null && slotUI.Slot.Item != null)
            ItemTooltip.Instance?.ShowTooltip(slotUI.Slot.Item);
        else
            ItemTooltip.Instance?.HideTooltip();
    }
    #endregion

    #region QuickSlot
    private void HandleQuickSlotPressed(int slotIndex)
    {
        if (inventory == null) return;

        if (slotIndex >= 0 && slotIndex < inventory.quickSlots.Count)
        {
            ItemSlot targetSlot = inventory.quickSlots[slotIndex];
            if (targetSlot != null && targetSlot.Item != null)
            {
                Debug.Log($"퀵슬롯 {slotIndex + 1}번 아이템 사용: {targetSlot.Item.ItemName}");
                UseSlotItem(targetSlot);
            }
        }
    }

    private void UseSlotItem(ItemSlot slot)
    {
        if (slot == null || slot.Item == null) return;

        Item usedItem = slot.Item;
        usedItem.Use(character);

        if (usedItem is UsableItem usable && usable.IsConsumable && ReferenceEquals(slot.Item, usedItem))
        {
            ConsumeOneFromSlot(slot);
        }
    }

    private void ConsumeOneFromSlot(ItemSlot slot)
    {
        if (slot == null || slot.Item == null) return;

        slot.Amount--;
        if (slot.Amount <= 0)
        {
            Item removedItem = slot.Item;
            slot.Item = null;
            slot.Amount = 0;
            removedItem.Destroy();
        }

        slot.UpdateSlot();
    }
    #endregion
}