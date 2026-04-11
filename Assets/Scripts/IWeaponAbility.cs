using UnityEngine;

public interface IWeaponAbility
{
    bool ExecuteAttack(Character player);
    bool IsAttacking { get; }
}