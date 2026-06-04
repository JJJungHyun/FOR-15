using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public abstract class BaseItemSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("UI References (Child Objects)")]
    [Tooltip("자식 오브젝트로 분리된 아이템 아이콘 이미지 컴포넌트")]
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

        if (iconImage != null)
        {
            iconImage.sprite = slot.Item.Icon;
            iconImage.enabled = true; // 아이콘 활성화
            Color c = iconImage.color;
            c.a = 1f; // 온전한 불투명
            iconImage.color = c;
        }

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

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false; // 아이템이 없으면 자식 아이콘 이미지를 꺼서 부모 슬롯만 보이게 함
        }

        if (amountText != null) amountText.enabled = false;
        if (durabilityBarObject != null) durabilityBarObject.SetActive(false);
    }

    private void HandleDurabilityChanged() => UpdateUI(slot);

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

    private void OnEnable() => _canvasGroup.blocksRaycasts = true;

    private void OnDisable()
    {
        if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip();
        if (StatTooltip.Instance != null) StatTooltip.Instance.HideTooltip();

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        OnPointerExit(pointerData);

        _canvasGroup.blocksRaycasts = false;
    }

    public void SetAlpha(float alpha)
    {
        if (iconImage != null && iconImage.enabled)
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