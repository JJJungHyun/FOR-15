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

    [Header("Durability Logic Select")]
    [SerializeField] private bool consumeOnFire = true; 
    [SerializeField] private int durabilityCost = 1;

    public float MaxChargeTime => maxChargeTime;
    public float AttackCooldown => attackCooldown;
    public bool IsAttacking => false;
    public bool IsCharging => false;
    public float ChargeRatio => 0f;

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
        if (projectilePrefab == null) return;

        PlayerEquipment equipment = player.GetComponent<PlayerEquipment>();
        EquippableItem currentWeapon = equipment != null ? equipment.CurrentSelectedWeapon : null;

        // [옵션 1] 활을 쏘는 행위 자체만으로 내구도 감소
        if (consumeOnFire && currentWeapon != null)
        {
            currentWeapon.ConsumeDurability(durabilityCost, player);
        }

        float finalRange = Mathf.Lerp(minRange, maxRange, chargeRatio);
        GameObject proj = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
        
        if (proj.TryGetComponent(out Projectile projectileScript))
        {
            // [옵션 2]
            EquippableItem weaponSource = consumeOnFire ? null : currentWeapon;
            projectileScript.Setup(attackDir, projectileSpeed, finalRange, player.GetAttackDamage());
            projectileScript.SetWeaponSource(weaponSource, player, durabilityCost);
        }
    }
}