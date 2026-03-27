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
        if (isInitialized)
        {
            HideAllTooltips();

            return;
        }
        isInitialized = true;

        if (itemSlots == null) itemSlots = new List<ItemSlot>();

        ClearOldUI();

        int defaultCount = Mathf.Max(0, totalInventorySize - quickSlotSize);

        for (int i = 0; i < defaultCount; i++) CreateSlotInList(defaultSlots, defaultAreaParent);
        for (int i = 0; i < quickSlotSize; i++) CreateSlotInList(quickSlots, quickAreaParent);

        itemSlots.AddRange(defaultSlots);
        itemSlots.AddRange(quickSlots);

        if (startingItems != null) SetStartingItems();

        OnInventoryInitialized?.Invoke();
    }

    private void ClearOldUI()
    {
        CleanTransform(defaultAreaParent);
        CleanTransform(quickAreaParent);

        itemSlots.Clear();
        defaultSlots.Clear();
        quickSlots.Clear();
    }

    private void CleanTransform(Transform parent)
    {
        if (parent == null) return;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void CreateSlotInList(List<ItemSlot> list, Transform parent)
    {
        ItemSlot newSlot = new ItemSlot();
        list.Add(newSlot);
        if (parent != null && slotPrefab != null)
        {
            var ui = Instantiate(slotPrefab, parent);
            ui.SetSlot(newSlot);
        }
    }

    // 아이템 획득 시 아이템 정렬
    public bool AddItemCustomPriority(Item item, int amount = 1)
    {
        EnsureInitialized();

        // 퀵슬롯 -> 일반슬롯
        if (TryStack(quickSlots, item, amount)) return true;
        if (TryStack(defaultSlots, item, amount)) return true;

        if (TryFill(quickSlots, item, amount)) return true;
        if (TryFill(defaultSlots, item, amount)) return true;

        Debug.LogWarning($"<color=red>[Inventory]</color> 인벤토리가 가득 찼습니다! '{item.name}'을(를) 더 이상 넣을 수 없습니다.");
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