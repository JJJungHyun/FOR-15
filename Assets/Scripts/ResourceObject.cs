using UnityEngine;

public class ResourceObject : MonoBehaviour, IResourceHarvestable
{
    [Header("자원 설정")]
    [SerializeField] private ToolType _requiredTool = ToolType.Axe; // 채집에 필요한 도구
    [SerializeField] private float _maxHp = 3f;                     // 기본 타격 횟수
    [SerializeField] private float _currentHp;

    [Header("도구 배율")]
    [SerializeField] private float _matchingToolDamage = 1f;       // 알맞은 도구 사용 시 차감될 체력
    [SerializeField] private float _wrongToolDamage = 0f;          // 잘못된 도구 사용 시 차감될 체력
    [SerializeField] private float _bareHandDamage = 0.5f;         // 맨손일 때 차감될 체력

    private ObjectFaller _faller;
    private ObjectFader _fader;
    private bool _isDestroyed = false;

    void Awake()
    {
        _currentHp = _maxHp;
        _faller = GetComponent<ObjectFaller>();
        _fader = GetComponent<ObjectFader>();
    }

    public void Harvest(float damage, ToolType toolType, Vector2 attackerPos)
    {
        if (_isDestroyed) return;

        float finalDamage = 0f;

        if (toolType == _requiredTool)
        {
            finalDamage = _matchingToolDamage;
            Debug.Log($"[파밍] 도구({toolType}) 대미지: {finalDamage}");
        }
        else if (toolType == ToolType.None)
        {
            finalDamage = _bareHandDamage;
            Debug.Log($"[파밍] 맨손 타격 대미지: {finalDamage}");
        }
        else
        {
            finalDamage = _wrongToolDamage;
            Debug.LogWarning($"[파밍] 해당 도구({toolType})로는 이 오브젝트를 캘 수 없습니다.");
        }

        if (finalDamage > 0)
        {
            _currentHp -= finalDamage;
            Debug.Log($"[파밍] 오브젝트 남은 체력: {_currentHp}/{_maxHp}");
        }

        if (_currentHp <= 0)
        {
            BreakObject();
        }
    }

    private void BreakObject()
    {
        _isDestroyed = true;

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;

        if (_faller != null) _faller.Fall();
        if (_fader != null) _fader.FadeOut(_faller != null ? _faller.FallTime : 0f);
    }
}