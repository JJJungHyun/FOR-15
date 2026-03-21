using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public abstract class BaseItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("UI References")]
    [SerializeField] protected Image iconImage;
    [SerializeField] protected TMP_Text amountText;

    protected ItemSlot _slot;
    public ItemSlot Slot => _slot;

    public static event Action<BaseItemSlotUI> OnSlotRightClickEvent;
    public static event Action<BaseItemSlotUI> OnSlotBeginDragEvent;
    public static event Action<BaseItemSlotUI> OnSlotEndDragEvent;
    public static event Action<BaseItemSlotUI> OnSlotDragEvent;
    public static event Action<BaseItemSlotUI> OnSlotDropEvent;

    public virtual void SetSlot(ItemSlot newSlot)
    {
        if (_slot != null) _slot.OnSlotChanged -= UpdateUI;
        _slot = newSlot;
        if (_slot != null)
        {
            _slot.OnSlotChanged += UpdateUI;
            UpdateUI(_slot);
        }
    }

    protected virtual void UpdateUI(ItemSlot slot)
    {
        if (slot != null && slot.Item != null)
        {
            iconImage.sprite = slot.Item.Icon;
            iconImage.color = Color.white;
            if (amountText != null)
            {
                amountText.text = slot.Amount > 1 ? slot.Amount.ToString() : "";
                amountText.enabled = slot.Amount > 1;
            }
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1, 1, 1, 0);
            if (amountText != null)
            {
                amountText.enabled = false;
            }
        }
    }

    public void SetAlpha(float alpha)
    {
        if (iconImage != null)
        {
            Color c = iconImage.color;
            c.a = alpha;
            iconImage.color = c;
        }
    }

    public virtual bool CanReceiveItem(Item item) => true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) OnSlotRightClickEvent?.Invoke(this);
    }
    public void OnBeginDrag(PointerEventData eventData) => OnSlotBeginDragEvent?.Invoke(this);
    public void OnDrag(PointerEventData eventData) => OnSlotDragEvent?.Invoke(this);
    public void OnEndDrag(PointerEventData eventData) => OnSlotEndDragEvent?.Invoke(this);
    public void OnDrop(PointerEventData eventData) => OnSlotDropEvent?.Invoke(this);
    public void OnPointerEnter(PointerEventData eventData) { }
    public void OnPointerExit(PointerEventData eventData) { }
}