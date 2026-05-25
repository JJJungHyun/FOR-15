using UnityEngine;

public interface IWeaponAbility
{
    void OnAttackStart(Character player, Vector3 attackDir);
    void OnAttackHold(Character player, Vector3 attackDir);
    void OnAttackRelease(Character player, Vector3 attackDir);

    bool IsAttacking { get; } 
    bool IsCharging { get; }  
    float ChargeRatio { get; } 
}