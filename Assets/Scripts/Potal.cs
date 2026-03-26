using UnityEngine;
using UnityEngine.InputSystem; // New Input System을 사용 중이므로 추가

public class Portal : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string _targetSceneName; // 이동할 씬 이름 (Build Settings와 일치해야 함)

    private bool _isPlayerInRange = false;

    void Update()
    {
        // 1. 플레이어가 범위 안에 있고
        // 2. E키를 이번 프레임에 눌렀다면
        if (_isPlayerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // MySceneManager의 싱글톤 인스턴스를 찾아 씬 전환 시작!
            if (MySceneManager.Instance != null)
            {
                MySceneManager.Instance.ChangeScene(_targetSceneName);
            }
            else
            {
                Debug.LogError("씬에 MySceneManager가 없습니다!");
            }
        }
    }

    // 플레이어 감지 범위 진입
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            Debug.Log("E키를 눌러 포탈 이용 가능");
        }
    }

    // 플레이어 감지 범위 이탈
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInRange = false;
        }
    }
}