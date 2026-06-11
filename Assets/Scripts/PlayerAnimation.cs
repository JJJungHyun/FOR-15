using UnityEngine;
using System;

public enum PlayerAnimState
{
    Idle = 0,
    MoveLeft,
    MoveDown,
    MoveUp,
    HandDAttack,
    HandLAttack,
    HandUAttack,
    WeaponDAttack,
    WeaponLAttack,
    WeaponUAttack
}

public class PlayerAnimation
{
    private Animator _animator;

    public Action<PlayerAnimState> OnSetAnimState;
    public Action<PlayerAnimState> OnPlayAnimState;
    public Action<string, bool> OnSetBool;

    public PlayerAnimation(Animator animator)
    {
        _animator = animator;
    }

    public void SetAnimState(PlayerAnimState animState)
    {
        _animator.SetInteger("animState", (int)animState);

        OnSetAnimState?.Invoke(animState);
    }

    public void PlayAnimState(PlayerAnimState animState)
    {
        _animator.Play(animState.ToString());

        OnPlayAnimState?.Invoke(animState);
    }

    public void SetBool(string param, bool value)
    {
        _animator.SetBool(param, value);

        OnSetBool?.Invoke(param, value);
    }
}