using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : ItemContainer
{
    [Header("Inventory Settings")]
    [SerializeField] private int totalInventorySize = 18;
    [SerializeField] private int quickSlotSize = 6;
    [SerializeField] private Item[] startingItems;

    [Header("UI Area Anchors")]
    [SerializeField] private Transform defaultAreaParent;
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

        int defaultCount = Mathf.Max(0, totalInventorySize - quickSlotSize);

        for (int i = 0; i < defaultCount; i++) CreateDataSlot(defaultSlots);
        for (int i = 0; i < quickSlotSize; i++) CreateDataSlot(quickSlots);

        itemSlots.AddRange(defaultSlots);
        itemSlots.AddRange(quickSlots);

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

        // 기존 UI 삭제
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