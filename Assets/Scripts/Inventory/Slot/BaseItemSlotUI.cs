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
    [SerializeField] protected GameObject durabilityBarObject;
    [SerializeField] protected Image durabilityFillImage;

    protected ItemSlot slot;

    private EquippableItem trackedEquipItem;
    private CanvasGroup _canvasGroup;

    public ItemSlot Slot => slot;
    public static event Action<BaseItemSlotUI> OnSlotRightClickEvent;
    public static event Action<BaseItemSlotUI> OnSlotBeginDragEvent;
    public static event Action<BaseItemSlotUI> OnSlotEndDragEvent;
    public static event Action<BaseItemSlotUI> OnSlotDragEvent;
    public static event Action<BaseItemSlotUI> OnSlotDropEvent;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public virtual void SetSlot(ItemSlot newSlot)
    {
        if (slot != null) slot.OnSlotChanged -= UpdateUI;

        UnbindDurabilityEvent();

        slot = newSlot;
        if (slot != null)
        {
            slot.OnSlotChanged += UpdateUI;
            UpdateUI(slot);
        }
    }

    protected virtual void UpdateUI(ItemSlot slot)
    {
        if (slot == null || slot.Item == null)
        {
            ClearSlotUI();
            return;
        }

        iconImage.sprite = slot.Item.Icon;
        iconImage.color = Color.white;

        if (amountText != null)
        {
            amountText.text = slot.Amount > 1 ? slot.Amount.ToString() : "";
            amountText.enabled = slot.Amount > 1;
        }

        if (slot.Item is EquippableItem equipItem && equipItem.HasDurability)
        {
            if (durabilityBarObject != null) durabilityBarObject.SetActive(true);

            float ratio = (float)equipItem.CurrentDurability / equipItem.MaxDurability;
            if (durabilityFillImage != null)
            {
                durabilityFillImage.fillAmount = ratio;
                durabilityFillImage.color = Color.Lerp(Color.red, Color.green, ratio);
            }

            UnbindDurabilityEvent();
            trackedEquipItem = equipItem;
            trackedEquipItem.OnDurabilityChanged += HandleDurabilityChanged;
        }
        else
        {
            UnbindDurabilityEvent();
            if (durabilityBarObject != null) durabilityBarObject.SetActive(false);
        }
    }

    private void ClearSlotUI()
    {
        UnbindDurabilityEvent();

        iconImage.sprite = null;
        iconImage.color = new Color(1, 1, 1, 0);

        if (amountText != null) amountText.enabled = false;

        if (durabilityBarObject != null) durabilityBarObject.SetActive(false);
    }

    private void HandleDurabilityChanged()
    {
        UpdateUI(slot);
    }

    private void UnbindDurabilityEvent()
    {
        if (trackedEquipItem != null)
        {
            trackedEquipItem.OnDurabilityChanged -= HandleDurabilityChanged;
            trackedEquipItem = null;
        }
    }

    private void OnDestroy()
    {
        if (slot != null) slot.OnSlotChanged -= UpdateUI;
        UnbindDurabilityEvent();
    }

    private void OnEnable()
    {
        if (_canvasGroup != null) _canvasGroup.blocksRaycasts = true;
    }

    private void OnDisable()
    {
        if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip();
        if (StatTooltip.Instance != null) StatTooltip.Instance.HideTooltip();

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        OnPointerExit(pointerData);

        if (_canvasGroup != null) _canvasGroup.blocksRaycasts = false;
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

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (slot != null && slot.Item != null && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.ShowTooltip(slot.Item);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip();
    }
}