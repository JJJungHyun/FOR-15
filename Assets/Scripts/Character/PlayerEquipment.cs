using UnityEngine;
using System.Collections;

public class PlayerEquipment : MonoBehaviour
{
    private Character player;
    private Inventory inventory;
    private InventoryController inventoryController;

    [Header("Weapon Placement")]
    [SerializeField] private Transform weaponHolder;

    private GameObject currentWeaponInstance;
    private IWeaponAbility currentWeaponAbility;
    private EquippableItem currentEquippedItem;
    private int currentSlotIndex = -1;

    public IWeaponAbility CurrentWeaponAbility => currentWeaponAbility;
    public GameObject CurrentWeaponInstance => currentWeaponInstance;
    public Transform WeaponHolder => weaponHolder;

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

        if (inventory != null && inventory.quickSlots.Count > 0)
        {
            if (currentSlotIndex == -1)
            {
                HandleQuickSlot(0);
            }
        }
    }

    private void OnEnable() => PlayerInputHandler.OnQuickSlotPressed += HandleQuickSlot;
    private void OnDisable() => PlayerInputHandler.OnQuickSlotPressed -= HandleQuickSlot;

    public void HandleQuickSlot(int index)
    {
        if (inventory == null) return;
        if (index < 0 || index >= inventory.quickSlots.Count) return;

        if (currentSlotIndex == index)
        {
            UnequipWeapon();
            currentSlotIndex = -1;
            return;
        }

        currentSlotIndex = index;
        ItemSlot selectedSlot = inventory.quickSlots[index];

        if (selectedSlot != null && selectedSlot.Item is EquippableItem equippable && equippable.EquipmentType == EquipmentType.Weapon)
            EquipWeapon(equippable);
        else
            UnequipWeapon();
    }

    private void EquipWeapon(EquippableItem newItem)
    {
        UnequipWeapon();
        currentEquippedItem = newItem;
        currentEquippedItem.Equip(player);

        if (newItem.WeaponPrefab != null)
        {
            currentWeaponInstance = Instantiate(newItem.WeaponPrefab, weaponHolder);
            currentWeaponInstance.transform.localPosition = Vector3.zero;
            currentWeaponInstance.transform.localRotation = Quaternion.identity;
            currentWeaponInstance.transform.localScale = Vector3.one;

            currentWeaponAbility = currentWeaponInstance.GetComponent<IWeaponAbility>();

            SpriteRenderer sr = currentWeaponInstance.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = newItem.Icon;

            if (currentWeaponInstance.TryGetComponent(out WeaponHandler handler))
                handler.UpdateColliderSize();
        }
    }

    private void UnequipWeapon()
    {
        if (currentEquippedItem != null) { currentEquippedItem.Unequip(player); currentEquippedItem = null; }
        if (currentWeaponInstance != null) { Destroy(currentWeaponInstance); currentWeaponAbility = null; }
    }
}