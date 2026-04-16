using UnityEngine;

public class ObjectColorFeedback : MonoBehaviour
{
    [Header("거리 설정")]
    [SerializeField] private float _interactRange = 3.0f; // 상호작용 가능 거리

    [Header("색상 설정")]
    [SerializeField] private Color _canInteractColor = Color.blue;   // 거리 안: 파란색
    [SerializeField] private Color _cannotInteractColor = Color.red; // 거리 밖: 빨간색

    private SpriteRenderer _spriteRenderer;
    private Color _originColor;
    private Transform _playerTransform;

    void Awake()
    {
        // GetComponent 대신 GetComponentInChildren을 사용합니다.
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _originColor = _spriteRenderer.color;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    // 마우스 커서가 오브젝트 위에 있을 때 매 프레임 실행
    private void OnMouseOver()
    {
        if (_playerTransform == null) return;

        // 플레이어와 오브젝트 사이의 거리 계산
        float distance = Vector2.Distance(_playerTransform.position, transform.position);

        if (distance <= _interactRange)
        {
            // 상호작용 가능 거리 안이면 파란색
            _spriteRenderer.color = _canInteractColor;
        }
        else
        {
            // 상호작용 불가능 거리면 빨간색
            _spriteRenderer.color = _cannotInteractColor;
        }
    }

    // 마우스 커서가 오브젝트를 벗어나면 원래 색으로
    private void OnMouseExit()
    {
        _spriteRenderer.color = _originColor;
    }
}