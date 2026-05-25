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

    // 데이터 조회용 프로퍼티 오픈
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

        float finalRange = Mathf.Lerp(minRange, maxRange, chargeRatio);

        GameObject proj = Instantiate(projectilePrefab, player.transform.position, Quaternion.identity);
        if (proj.TryGetComponent(out Projectile projectileScript))
        {
            projectileScript.Setup(attackDir, projectileSpeed, finalRange, player.GetAttackDamage());
        }
    }
}