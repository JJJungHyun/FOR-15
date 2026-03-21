using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Character character;
    [SerializeField] private Image dragIcon; // 마우스 따라다니는 이미지

    private BaseItemSlotUI _draggedSlot;

    private void OnEnable()
    {
        PlayerInputHandler.OnInventoryPressed += ToggleInventory;

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
    }

    private void HandleBeginDrag(BaseItemSlotUI slotUI)
    {
        if (slotUI.Slot.Item == null) return;

        _draggedSlot = slotUI;
        _draggedSlot.SetAlpha(0.5f);
        dragIcon.sprite = slotUI.Slot.Item.Icon;
        dragIcon.SetNativeSize();
        dragIcon.gameObject.SetActive(true);
        UpdateDragIconPosition();
    }

    private void HandleDrag(BaseItemSlotUI slotUI) => UpdateDragIconPosition();

    private void UpdateDragIconPosition()
    {
        if (_draggedSlot != null && Input.mousePosition != null)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    private void HandleEndDrag(BaseItemSlotUI slotUI)
    {
        if (_draggedSlot != null)
            _draggedSlot.SetSlot(_draggedSlot.Slot);

        _draggedSlot = null;
        dragIcon.gameObject.SetActive(false);
    }

    private void HandleDrop(BaseItemSlotUI dropSlotUI)
    {
        if (_draggedSlot == null || dropSlotUI == null) return;

        if (_draggedSlot.Slot == null || dropSlotUI.Slot == null)
        {
            Debug.LogWarning("슬롯에 데이터(ItemSlot)가 연결되지 않았습니다.");
            return;
        }

        if (dropSlotUI.CanReceiveItem(_draggedSlot.Slot.Item) &&
            _draggedSlot.CanReceiveItem(dropSlotUI.Slot.Item))
        {
            ItemSlot source = _draggedSlot.Slot;
            ItemSlot target = dropSlotUI.Slot;

            Item tempItem = source.Item;
            int tempAmount = source.Amount;

            source.Item = target.Item;
            source.Amount = target.Amount;

            target.Item = tempItem;
            target.Amount = tempAmount;

            source.UpdateSlot();
            target.UpdateSlot();
        }


    }

    private void HandleRightClick(BaseItemSlotUI slotUI)
    {
        Item item = slotUI.Slot.Item;
        if (item == null) return;

        if (item is EquippableItem equippable) equippable.Equip(character);
        else if (item is UsableItem usable)
        {
            usable.Use(character);
            if (usable.IsConsumable) inventory.RemoveItem(item);
        }
    }

    private void ToggleInventory()
    {
        bool isActive = inventory.gameObject.activeSelf;
        inventory.gameObject.SetActive(!isActive);

        Cursor.visible = !isActive;
        Cursor.lockState = !isActive ? CursorLockMode.None : CursorLockMode.Locked;

        if (!isActive && _draggedSlot != null)
        {
            HandleEndDrag(_draggedSlot);
        }
    }
}