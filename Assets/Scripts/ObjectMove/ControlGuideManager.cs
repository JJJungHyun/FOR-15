using UnityEngine;
using UnityEngine.InputSystem; // 새 입력 시스템
using DG.Tweening;

public class ControlGuideManager : MonoBehaviour
{
    public static ControlGuideManager Instance;

    [Header("UI 연결")]
    [SerializeField] private RectTransform _guideRect;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("애니메이션 설정")]
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _moveDistance = 100f;

    private Vector2 _targetPos;
    private Vector2 _hiddenPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _targetPos = _guideRect.anchoredPosition;
        _hiddenPos = _targetPos + new Vector2(0, -_moveDistance);

        _guideRect.anchoredPosition = _hiddenPos;
        _canvasGroup.alpha = 0;
    }

    void Update()
    {
        // --- Tab 키 대신 Q 키를 사용하도록 수정되었습니다 ---

        // 1. Q 키를 누르는 순간 (등장)
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            ShowGuide();
        }

        // 2. Q 키를 떼는 순간 (퇴장)
        if (Keyboard.current.qKey.wasReleasedThisFrame)
        {
            HideGuide();
        }
    }

    private void ShowGuide()
    {
        _guideRect.DOKill();
        _canvasGroup.DOKill();

        _guideRect.DOAnchorPos(_targetPos, _duration).SetEase(Ease.OutQuart);
        _canvasGroup.DOFade(1f, _duration);
    }

    private void HideGuide()
    {
        _guideRect.DOKill();
        _canvasGroup.DOKill();

        _guideRect.DOAnchorPos(_hiddenPos, _duration).SetEase(Ease.InQuart);
        _canvasGroup.DOFade(0f, _duration);
    }
}