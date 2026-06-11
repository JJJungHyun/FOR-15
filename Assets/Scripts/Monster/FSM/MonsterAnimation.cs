using UnityEngine;

public enum MonsterAnimState
{
    Idle = 0,
    Walk = 1,
    Die = 2,
    Attack = 3
}

public class MonsterAnimation
{
    private Animator _animator;
    private SpriteRenderer _renderer;

    private Vector2 _lastDirection = Vector2.down;
    private MonsterAnimState _currentState = MonsterAnimState.Idle;

    public MonsterAnimation(Animator animator, SpriteRenderer renderer)
    {
        _animator = animator;
        _renderer = renderer;
    }

    public void UpdateMoveAnimation(Vector3 direction)
    {
        if (_animator == null || _currentState == MonsterAnimState.Die) return;

        if (_currentState == MonsterAnimState.Attack) return;

        if (direction.sqrMagnitude < 0.001f)
        {
            SetAnimState(MonsterAnimState.Idle);
            return;
        }

        UpdateDirection(direction);

        SetAnimState(MonsterAnimState.Walk);
    }

    public void PlayAttackAnimation(Vector3 targetDirection)
    {
        if (_animator == null || _currentState == MonsterAnimState.Die) return;

        UpdateDirection(targetDirection);

        SetAnimState(MonsterAnimState.Attack);
    }

    private void UpdateDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        Vector2 normDir = direction.normalized;

        if (Mathf.Abs(normDir.y) > Mathf.Abs(normDir.x))
        {
            _lastDirection = normDir.y > 0 ? Vector2.up : Vector2.down;
            _renderer.flipX = false;
        }
        else
        {
            _lastDirection = normDir.x > 0 ? Vector2.right : Vector2.left;
            _renderer.flipX = (normDir.x > 0);
        }

        _animator.SetFloat("DirX", _lastDirection.x);
        _animator.SetFloat("DirY", _lastDirection.y);
    }

    public void SetAnimState(MonsterAnimState state)
    {
        if (_animator == null) return;

        _currentState = state;

        _animator.SetFloat("DirX", _lastDirection.x);
        _animator.SetFloat("DirY", _lastDirection.y);

        _animator.SetInteger("animState", (int)state);
    }
}