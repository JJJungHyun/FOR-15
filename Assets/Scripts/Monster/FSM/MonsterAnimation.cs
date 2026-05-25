using UnityEngine;

public enum MonsterAnimState
{
    Idle = 0,
    Walk = 1,
    Die = 2
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

        // 1. 멈춤 체크
        if (direction.sqrMagnitude < 0.001f)
        {
            SetAnimState(MonsterAnimState.Idle);
            return;
        }

        // 2. 방향성 계산 먼저 수행
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

        // 3. 파라미터 전달 순서 최적화 (방향을 먼저 꽂고 상태를 변경)
        _animator.SetFloat("DirX", _lastDirection.x);
        _animator.SetFloat("DirY", _lastDirection.y);

        SetAnimState(MonsterAnimState.Walk);
    }

    public void SetAnimState(MonsterAnimState state)
    {
        if (_animator == null) return;

        _currentState = state;

        // 방향 값을 확실하게 먼저 먹이고 상태 인티저를 변경합니다.
        _animator.SetFloat("DirX", _lastDirection.x);
        _animator.SetFloat("DirY", _lastDirection.y);

        _animator.SetInteger("animState", (int)state);
    }
}