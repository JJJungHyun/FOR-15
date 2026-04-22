using UnityEngine;

public enum WolfAnimState
{
    MoveLeft = 0,
    MoveUp,
    MoveDown
}

public class WolfAnimation
{
    private Animator _animator;

    public WolfAnimation(Animator animator)
    {
        _animator = animator;
    }

    public void SetAnimState(WolfAnimState animState)
    {
        _animator.SetInteger("animState", (int)animState);
    }

    public void PlayAnimState(WolfAnimState animState)
    {
        _animator.Play(animState.ToString());
    }
}