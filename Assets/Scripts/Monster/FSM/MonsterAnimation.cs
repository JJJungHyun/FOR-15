using UnityEngine;

public enum MonsterAnimState
{
    Idle = -1,   
    MoveLeft = 0,
    MoveUp = 1,
    MoveDown = 2,
    Die = 3      
}

public class MonsterAnimation
{
    private Animator _animator;
    private SpriteRenderer _renderer;

    public MonsterAnimation(Animator animator, SpriteRenderer renderer)
    {
        _animator = animator;
        _renderer = renderer;
    }

    public void UpdateMoveAnimation(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            _renderer.flipX = false;
            SetAnimState(direction.y > 0 ? MonsterAnimState.MoveUp : MonsterAnimState.MoveDown);
        }
        else
        {
            _renderer.flipX = (direction.x > 0); 
            SetAnimState(MonsterAnimState.MoveLeft);
        }
    }

    public void SetAnimState(MonsterAnimState state)
    {
        if (_animator == null) return;
        _animator.SetInteger("animState", (int)state);
    }
}