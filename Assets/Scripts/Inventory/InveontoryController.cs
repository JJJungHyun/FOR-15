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
        BaseItemSlotUI.OnSlotRightClickEvent += HandleRightClick;
        BaseItemSlotUI.OnSlotBeginDragEvent += HandleBeginDrag;
        BaseItemSlotUI.OnSlotEndDragEvent += HandleEndDrag;
        BaseItemSlotUI.OnSlotDragEvent += HandleDrag;
        BaseItemSlotUI.OnSlotDropEvent += HandleDrop;
        PlayerInputHandler.OnQuickSlotPressed += HandleQuickSlotPressed;
    }

    private void OnDisable()
    {
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

        // 서로 교환 가능한 아이템 구조인지만 체크 (순수 스왑 기능)
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
        Item item = slotUI.Slot.Item;
        if (item == null) return;

        // [장비 장착/해제 로직 제거됨] 
        // 이제 인벤토리는 소모품 등 공통 사용 로직만 보유합니다.
        if (item is UsableItem usable)
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
                targetSlot.Item.Use(character);
            }
        }
    }
    #endregion
}