using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Abilities/Ranged Ability")]
public class RangedAbility : ScriptableObject, IWeaponAbility
{
    [Header("Ranged Config")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private float minRange = 2f;
    [SerializeField] private float maxRange = 12f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Ammo Settings (SO Direct Assignment)")]
    [SerializeField] private Item requiredAmmoItem;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private float soundVolume = 1f;

    [Header("Durability Logic Select")]
    [SerializeField] private bool consumeOnFire = true;
    [SerializeField] private int durabilityCost = 1;

    public float MaxChargeTime => maxChargeTime;
    public float AttackCooldown => attackCooldown;
    public bool IsAttacking => false;
    public bool IsCharging => false;
    public float ChargeRatio => 0f;

    public bool HasAmmo(Character player)
    {
        if (requiredAmmoItem == null) return true;

        InventoryController invController = player.GetComponentInChildren<InventoryController>();
        if (invController == null) return false;

        Inventory inventory = invController.GetInventory();
        if (inventory == null) return false;

        return inventory.ItemCount(requiredAmmoItem.ID) > 0;
    }

    public void OnAttackStart(Character player, Vector3 attackDir) { }
    public void OnAttackHold(Character player, Vector3 attackDir) { }

    public void OnAttackRelease(Character player, Vector3 attackDir)
    {
        if (player.TryGetComponent(out PlayerCombat combat))
        {
            Fire(player, attackDir, combat.ChargeRatio);
        }
    }

    private void Fire(Character player, Vector3 attackDir, float chargeRatio)
    {
        InventoryController invController = player.GetComponentInChildren<InventoryController>();
        if (invController == null) return;

        Inventory inventory = invController.GetInventory();
        if (inventory == null) return;

        Sprite ammoSprite = null;

        if (requiredAmmoItem != null)
        {
            if (!HasAmmo(player))
            {
                Debug.LogWarning($"탄환이 부족합니다! 필요한 탄환: {requiredAmmoItem.ItemName}");
                return;
            }

            string targetAmmoID = requiredAmmoItem.ID;
            ammoSprite = requiredAmmoItem.Icon;

            inventory.RemoveItemByID(targetAmmoID);
            invController.RefreshAllSlots();
        }

        if (projectilePrefab == null) return;

        PlayerEquipment equipment = player.GetComponent<PlayerEquipment>();
        EquippableItem currentWeapon = equipment != null ? equipment.CurrentSelectedWeapon : null;

        if (consumeOnFire && currentWeapon != null)
        {
            currentWeapon.ConsumeDurability(durabilityCost, player);
        }

        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, player.transform.position, soundVolume);
        }

        float finalRange = Mathf.Lerp(minRange, maxRange, chargeRatio);
        GameObject proj = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);

        if (proj.TryGetComponent(out Projectile projectileScript))
        {
            EquippableItem weaponSource = consumeOnFire ? null : currentWeapon;
            projectileScript.Setup(attackDir, projectileSpeed, finalRange, player.GetAttackDamage(), ammoSprite);
            projectileScript.SetWeaponSource(weaponSource, player, durabilityCost);
        }
    }
}