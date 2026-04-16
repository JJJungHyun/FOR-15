using UnityEngine;
using UnityEngine.InputSystem; // 새 입력 시스템

public class PersistentCursor : MonoBehaviour
{
    // 어디서든 하나만 존재하도록 싱글톤 설정
    public static PersistentCursor Instance;

    [SerializeField] private RectTransform _cursorRect;

    private void Awake()
    {
        // 1. 싱글톤 로직: 이미 커서가 있다면 새로 생긴 커서를 파괴함
        if (Instance == null)
        {
            Instance = this;
            // 2. 씬이 넘어가도 이 오브젝트를 파괴하지 않음 (최상위 부모여야 함)
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 시스템 커서 숨기기
        Cursor.visible = false;
    }

    void Update()
    {
        if (_cursorRect == null) return;

        // 마우스 좌표 읽기
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // 커서 이미지 위치 업데이트
        _cursorRect.position = mousePosition;
    }

    private void OnDisable()
    {
        // 게임 종료 시 커서 복구
        Cursor.visible = true;
    }
}