using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Abilities/Melee Ability")]
public class MeleeAbility : ScriptableObject, IWeaponAbility
{
    [Header("Melee Config")]
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private Vector2 attackBoxSize = new Vector2(1.5f, 1.2f);
    [SerializeField] private LayerMask damageableLayer;

    [Header("Durability Cost Config")]
    [SerializeField] private int costOnHitMonster = 1;      // 몬스터 타격 시 기본 소모량
    [SerializeField] private int costOnHarvest = 1;         // 자원 파밍 시 소모량
    [SerializeField] private int costOnHitMonsterWithTool = 3; //도구로 몬스터를 때렸을 때 패널티 소모량

    public float AttackCooldown => attackCooldown;
    public bool IsAttacking => false;
    public bool IsCharging => false;
    public float ChargeRatio => 0f;

    public void OnAttackStart(Character player, Vector3 attackDir)
    {
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger(DetermineDirectionTrigger(attackDir));
        }

        PerformOverlapAttack(player, attackDir);
    }

    public void OnAttackHold(Character player, Vector3 attackDir) { }
    public void OnAttackRelease(Character player, Vector3 attackDir) { }

    private string DetermineDirectionTrigger(Vector3 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? "Attack_Right" : "Attack_Left";
        else
            return dir.y > 0 ? "Attack_Up" : "Attack_Down";
    }

    private void PerformOverlapAttack(Character player, Vector3 attackDir)
    {
        Vector2 attackCenter = (Vector2)player.transform.position + ((Vector2)attackDir * attackRange);
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(attackCenter, attackBoxSize, angle, damageableLayer);
        float damage = player.GetAttackDamage();

        PlayerEquipment equipment = player.GetComponent<PlayerEquipment>();
        EquippableItem currentWeapon = equipment != null ? equipment.CurrentSelectedWeapon : null;

        bool hasHitAnything = false;

        foreach (Collider2D targetCollider in hitTargets)
        {
            if (targetCollider.gameObject == player.gameObject) continue;

            if (targetCollider.TryGetComponent(out IResourceHarvestable resource))
            {
                hasHitAnything = true;
                ToolType currentToolType = currentWeapon != null ? currentWeapon.ToolType : ToolType.None;
                resource.Harvest(damage, currentToolType, player.transform.position);

                if (currentWeapon != null)
                {
                    currentWeapon.ConsumeDurability(costOnHarvest, player);
                }
                break; 
            }

            if (targetCollider.TryGetComponent(out IDamageable target))
            {
                hasHitAnything = true;
                target.TakeDamage(damage, player.transform.position);

                if (currentWeapon != null)
                {
                    int finalCost = (currentWeapon.ToolType == ToolType.Axe || currentWeapon.ToolType == ToolType.Pickaxe)
                        ? costOnHitMonsterWithTool
                        : costOnHitMonster;

                    currentWeapon.ConsumeDurability(finalCost, player);
                }
            }
        }

    }
}