using UnityEngine;

public interface IWeaponAbility
{
    void OnAttackStart(Character player);    
    void OnAttackHold(Character player);    
    void OnAttackRelease(Character player); 
    bool IsAttacking { get; }
}