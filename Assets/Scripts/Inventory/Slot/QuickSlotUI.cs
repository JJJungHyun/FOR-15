using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private BaseItemSlotUI slotPrefab;
    [SerializeField] private Transform container;

    private bool isHUDInitialized = false;

    private void Awake()
    {
        if (inventory == null) return;
        inventory.OnInventoryInitialized += RefreshHUD;
    }

    private void Start()
    {
        if (inventory == null) return;

        inventory.EnsureInitialized();

        if (inventory.quickSlots != null && inventory.quickSlots.Count > 0)
        {
            RefreshHUD();
        }
    }

    private void RefreshHUD()
    {
        if (isHUDInitialized) return;
        if (inventory.quickSlots == null || inventory.quickSlots.Count == 0) return;

        isHUDInitialized = true;

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

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryInitialized -= RefreshHUD;
    }
}