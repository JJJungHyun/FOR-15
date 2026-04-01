using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    private Character player;
    private Inventory inventory;
    private InventoryController inventoryController;

    [Header("Weapon Placement")]
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private float holderOffset = 0.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackPreDelay = 0.1f;  // 휘두르기 전 대기
    [SerializeField] private float attackDuration = 0.15f; // 공격 판정 유지 시간
    [SerializeField] private float attackCooldown = 0.4f;  // 다음 공격 가능까지 총 시간

    private GameObject currentWeaponInstance;
    private WeaponHandler currentWeaponHandler;
    private EquippableItem currentEquippedItem;
    private int currentSlotIndex = -1;
    private bool isAttacking = false;

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
            if (inventory != null) inventory.EnsureInitialized();
        }
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnQuickSlotPressed += HandleQuickSlot;
        PlayerInputHandler.OnAttackPressed += TryAttack;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnQuickSlotPressed -= HandleQuickSlot;
        PlayerInputHandler.OnAttackPressed -= TryAttack;
    }

    private void Update()
    {
        HandleWeaponFacing();
    }

    private void HandleWeaponFacing()
    {
        if (weaponHolder == null || Camera.main == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - transform.position).normalized;

        weaponHolder.rotation = Quaternion.identity;

        Vector3 scale = Vector3.one;
        Vector3 pos = Vector3.zero;

        if (direction.x > 0)
        {
            scale.x = -1;
            pos.x = holderOffset;
        }
        else
        {
            scale.x = 1;
            pos.x = -holderOffset;
        }

        pos.y = 0;

        weaponHolder.localScale = scale;
        weaponHolder.localPosition = pos;

        AlwaysShowWeaponOnTop();
    }

    private void AlwaysShowWeaponOnTop()
    {
        if (currentWeaponInstance == null) return;

        SpriteRenderer weaponSr = currentWeaponInstance.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSr = GetComponent<SpriteRenderer>();

        if (weaponSr != null && playerSr != null)
        {
            weaponSr.sortingOrder = playerSr.sortingOrder + 1;
        }
    }

    public void TryAttack()
    {
        if (currentWeaponHandler == null || isAttacking) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 선딜레이
        yield return new WaitForSeconds(attackPreDelay);

        // 공격 판정 활성화 
        currentWeaponHandler.EnableAttack(player.GetAttackDamage());

        yield return new WaitForSeconds(attackDuration);

        currentWeaponHandler.DisableAttack();

        // 후딜레이 및 쿨타임 대기
        float remainCooldown = attackCooldown - attackPreDelay - attackDuration;
        if (remainCooldown > 0)
            yield return new WaitForSeconds(remainCooldown);

        isAttacking = false;
    }


    private void HandleQuickSlot(int index)
    {
        if (inventory == null && inventoryController != null)
            inventory = inventoryController.GetInventory();
        if (inventory == null) return;
        inventory.EnsureInitialized();

        if (index < 0 || index >= inventory.quickSlots.Count) return;
        if (currentSlotIndex == index) { UnequipWeapon(); currentSlotIndex = -1; return; }

        currentSlotIndex = index;
        ItemSlot selectedSlot = inventory.quickSlots[index];
        if (selectedSlot != null && selectedSlot.Item is EquippableItem equippable && equippable.EquipmentType == EquipmentType.Weapon)
            EquipWeapon(equippable);
        else UnequipWeapon();
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
            SpriteRenderer sr = currentWeaponInstance.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = newItem.Icon;
            currentWeaponHandler = currentWeaponInstance.GetComponent<WeaponHandler>();

            if (currentWeaponHandler != null)
                currentWeaponHandler.UpdateColliderSize();
        }
    }

    private void UnequipWeapon()
    {
        if (currentEquippedItem != null) { currentEquippedItem.Unequip(player); currentEquippedItem = null; }
        if (currentWeaponInstance != null) { Destroy(currentWeaponInstance); currentWeaponHandler = null; }
        isAttacking = false; 
    }
}