using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : ItemContainer
{
    [Header("Inventory Size Settings")]
    [Tooltip("순수 인벤토리 가방 내부의 총 칸 수 (예: 10칸씩 4줄 = 40)")]
    [SerializeField] private int totalInventorySize = 40;

    [Tooltip("인게임 항상 켜져있는 HUD 퀵슬롯 바의 칸 수")]
    [SerializeField] private int quickSlotSize = 10;

    [SerializeField] private Item[] startingItems;

    [Header("UI Area Anchors")]
    [Tooltip("인벤토리 UI 창 내부 Grid 패널 Parent")]
    [SerializeField] private Transform defaultAreaParent;

    [Tooltip("인게임 상시 HUD 퀵슬롯 바 Grid 패널 Parent")]
    [SerializeField] private Transform quickAreaParent;

    [SerializeField] private BaseItemSlotUI slotPrefab;

    public List<ItemSlot> defaultSlots = new List<ItemSlot>();
    public List<ItemSlot> quickSlots = new List<ItemSlot>();
    public event Action OnInventoryInitialized;

    private bool isInitialized = false;
    private GraphicRaycaster raycaster;

    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        EnsureInitialized();
    }

    private void OnEnable()
    {
        if (raycaster != null)
        {
            StopAllCoroutines();
            StartCoroutine(EnableRaycasterRoutine());
        }
        HideAllTooltips();
    }

    private IEnumerator EnableRaycasterRoutine()
    {
        raycaster.enabled = false;
        yield return null;
        raycaster.enabled = true;
    }

    private void HideAllTooltips()
    {
        ItemTooltip.Instance?.HideTooltip();
        StatTooltip.Instance?.HideTooltip();
    }

    public void EnsureInitialized()
    {
        if (isInitialized) return;
        isInitialized = true;

        if (itemSlots == null) itemSlots = new List<ItemSlot>();

        itemSlots.Clear();
        defaultSlots.Clear();
        quickSlots.Clear();

        // 🛠️ 마크 구조 변경: 가방 크기와 퀵슬롯 크기를 차감하지 않고 완전 독립적으로 생성
        for (int i = 0; i < totalInventorySize; i++) CreateDataSlot(defaultSlots);
        for (int i = 0; i < quickSlotSize; i++) CreateDataSlot(quickSlots);

        // 전체 컨테이너 통제 목록에는 가방 데이터와 퀵슬롯 데이터를 둘 다 연결 (총합 5줄 분량)
        itemSlots.AddRange(defaultSlots);
        itemSlots.AddRange(quickSlots);

        // 독립된 개별 부모 UI Grid 영역에 프리랩 생성 및 바인딩
        CreateSlotUI(defaultSlots, defaultAreaParent);
        CreateSlotUI(quickSlots, quickAreaParent);

        if (startingItems != null) SetStartingItems();

        OnInventoryInitialized?.Invoke();
    }

    private void CreateDataSlot(List<ItemSlot> list)
    {
        ItemSlot newSlot = new ItemSlot();
        list.Add(newSlot);
    }

    private void CreateSlotUI(List<ItemSlot> list, Transform parent)
    {
        if (parent == null || slotPrefab == null) return;

        foreach (Transform child in parent) Destroy(child.gameObject);

        foreach (var slot in list)
        {
            var ui = Instantiate(slotPrefab, parent);
            ui.SetSlot(slot);
        }
    }

    public bool AddItemCustomPriority(Item item, int amount = 1)
    {
        EnsureInitialized();

        // 아이템 루팅 시 마크처럼 퀵슬롯에 먼저 들어가도록 배치
        if (TryStack(quickSlots, item, amount)) return true;
        if (TryStack(defaultSlots, item, amount)) return true;
        if (TryFill(quickSlots, item, amount)) return true;
        if (TryFill(defaultSlots, item, amount)) return true;

        Debug.LogWarning($"인벤토리가 가득 찼습니다! '{item.name}'");
        return false;
    }

    private bool TryStack(List<ItemSlot> slots, Item item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.Item != null && slot.Item.ID == item.ID && slot.Amount < item.MaximumStacks)
            {
                slot.Amount += amount;
                slot.UpdateSlot();
                return true;
            }
        }
        return false;
    }

    private bool TryFill(List<ItemSlot> slots, Item item, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.Item == null)
            {
                slot.Item = item.GetCopy();
                slot.Amount = amount;
                slot.UpdateSlot();
                return true;
            }
        }
        return false;
    }

    private void SetStartingItems()
    {
        foreach (Item item in startingItems)
        {
            if (item != null) AddItemCustomPriority(item);
        }
    }
}