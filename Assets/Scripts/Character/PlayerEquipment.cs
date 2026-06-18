using System.Collections;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    private Character player;
    private Inventory inventory;
    private InventoryController inventoryController;

    private EquippableItem currentSelectedWeapon;
    private ItemSlot currentSelectedSlot;
    private int currentSlotIndex = -1;

    public EquippableItem CurrentSelectedWeapon => currentSelectedWeapon;

    private void Awake()
    {
        player = GetComponent<Character>();
        inventoryController = GetComponent<InventoryController>();
    }

    private void Start()
    {
        if (inventoryController == null) return;

        inventory = inventoryController.GetInventory();
        if (inventory != null)
        {
            inventory.EnsureInitialized();
            StartCoroutine(InitializeDefaultSlot());
        }
    }

    private IEnumerator InitializeDefaultSlot()
    {
        yield return null;
        HandleQuickSlot(0);
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnQuickSlotPressed += HandleQuickSlot;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnQuickSlotPressed -= HandleQuickSlot;
        UnbindSelectedSlot();
    }

    public void HandleQuickSlot(int index)
    {
        if (inventory == null) return;
        if (index < 0 || index >= inventory.quickSlots.Count) return;

        if (currentSlotIndex == index)
        {
            ClearCurrentWeaponSelection();
            return;
        }

        ClearCurrentWeaponSelection(false);

        currentSlotIndex = index;
        ItemSlot selectedSlot = inventory.quickSlots[index];

        if (selectedSlot != null && selectedSlot.Item is EquippableItem equippable && equippable.EquipmentType == EquipmentType.Weapon)
        {
            currentSelectedSlot = selectedSlot;
            currentSelectedSlot.OnSlotChanged += HandleSelectedSlotChanged;

            currentSelectedWeapon = equippable;
            currentSelectedWeapon.Equip(player);
        }

        UpdatePlayerAnimationState(currentSelectedWeapon);
    }

    private void HandleSelectedSlotChanged(ItemSlot changedSlot)
    {
        if (changedSlot != currentSelectedSlot) return;
        if (ReferenceEquals(changedSlot.Item, currentSelectedWeapon)) return;

        ClearCurrentWeaponSelection();
    }

    private void ClearCurrentWeaponSelection(bool updateAnimation = true)
    {
        RemoveCurrentWeaponStats();
        UnbindSelectedSlot();

        currentSlotIndex = -1;
        currentSelectedWeapon = null;

        if (updateAnimation)
        {
            UpdatePlayerAnimationState(null);
        }
    }

    private void UnbindSelectedSlot()
    {
        if (currentSelectedSlot != null)
        {
            currentSelectedSlot.OnSlotChanged -= HandleSelectedSlotChanged;
            currentSelectedSlot = null;
        }
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
            int toolType = weapon != null ? (int)weapon.ToolType : 0;
            anim.SetInteger("EquippedToolType", toolType);
        }
    }
}
