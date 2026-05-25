using UnityEngine;
using System.Collections;

public class PlayerEquipment : MonoBehaviour
{
    private Character player;
    private Inventory inventory;
    private InventoryController inventoryController;

    private EquippableItem currentSelectedWeapon;
    private int currentSlotIndex = -1;

    public EquippableItem CurrentSelectedWeapon => currentSelectedWeapon;

    private void Awake()
    {
        player = GetComponent<Character>();
        inventoryController = GetComponent<InventoryController>();
    }

    private void Start()
    {
        if (inventoryController != null)
        {
            inventory = inventoryController.GetInventory();
            if (inventory != null)
            {
                inventory.EnsureInitialized();
                StartCoroutine(InitializeDefaultSlot());
            }
        }
    }

    private IEnumerator InitializeDefaultSlot()
    {
        yield return null;
        HandleQuickSlot(0);
    }

    private void OnEnable() => PlayerInputHandler.OnQuickSlotPressed += HandleQuickSlot;
    private void OnDisable() => PlayerInputHandler.OnQuickSlotPressed -= HandleQuickSlot;

    public void HandleQuickSlot(int index)
    {
        if (inventory == null) return;
        if (index < 0 || index >= inventory.quickSlots.Count) return;

        if (currentSlotIndex == index)
        {
            RemoveCurrentWeaponStats();
            currentSlotIndex = -1;
            currentSelectedWeapon = null;
            UpdatePlayerAnimationState(null);
            return;
        }

        RemoveCurrentWeaponStats();

        currentSlotIndex = index;
        ItemSlot selectedSlot = inventory.quickSlots[index];

        if (selectedSlot != null && selectedSlot.Item is EquippableItem equippable && equippable.EquipmentType == EquipmentType.Weapon)
        {
            currentSelectedWeapon = equippable;
            currentSelectedWeapon.Equip(player);
        }
        else
        {
            currentSelectedWeapon = null;
        }

        UpdatePlayerAnimationState(currentSelectedWeapon);
    }

    private void RemoveCurrentWeaponStats()
    {
        if (currentSelectedWeapon != null)
        {
            currentSelectedWeapon.Unequip(player);
        }
    }

    private void UpdatePlayerAnimationState(EquippableItem weapon)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            int toolType = (weapon != null) ? (int)weapon.ToolType : 0;
            anim.SetInteger("EquippedToolType", toolType);
        }
    }
}