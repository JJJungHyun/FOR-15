using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Abilities/Melee Attack")]
public class MeleeAbility : ScriptableObject, IWeaponAbility
{
    [SerializeField] private float attackCooldown = 0.3f;
    private float lastAttackTime;

    public bool IsAttacking => Time.time < lastAttackTime + attackCooldown;

    public void OnAttackStart(Character player)
    {
        if (IsAttacking) return;

        lastAttackTime = Time.time;
    }

    public void OnAttackHold(Character player) { }
    public void OnAttackRelease(Character player) { }
}