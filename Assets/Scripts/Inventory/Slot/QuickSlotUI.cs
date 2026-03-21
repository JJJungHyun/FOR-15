using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private BaseItemSlotUI slotPrefab;
    [SerializeField] private Transform container;
    private bool _isHUDInitialized = false;

    private void Start()
    {
        if (inventory == null) return;

        inventory.OnInventoryInitialized += RefreshHUD;

        if (inventory.quickSlots.Count > 0)
        {
            RefreshHUD();
        }
    }

    private void RefreshHUD()
    {
        if (_isHUDInitialized) return; 
        _isHUDInitialized = true;

        // 기존 UI 청소
        if (container != null)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
                Destroy(container.GetChild(i).gameObject);
        }

        foreach (var dataSlot in inventory.quickSlots)
        {
            if (slotPrefab == null) break;
            var ui = Instantiate(slotPrefab, container);
            ui.SetSlot(dataSlot);
        }
    }
}