using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요

public class GameSceneInitializer : MonoBehaviour
{
    public static GameSceneInitializer Instance;

    private void Awake()
    {
        // --- 싱글톤 및 씬 전환 시 유지 설정 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 다른 씬으로 넘어가도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 존재한다면 새로 생긴 것은 파괴
            return;
        }
    }

    private void OnEnable()
    {
        // 씬이 로드될 때마다 OnSceneLoaded 함수가 실행되도록 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 오브젝트가 사라질 때 이벤트 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 바뀔 때마다 자동으로 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 타이틀 씬(예: "TitleScene")에서는 로드하지 않도록 조건 추가 가능
        if (scene.name == "TitleScene") return;

        // 이어하기 모드라면 매니저에게 로드를 시킴
        if (GameSaveManager.Instance != null && GameSaveManager.Instance.isContinueGame)
        {
            Debug.Log($"{scene.name} 씬 로딩 완료: 데이터를 불러옵니다.");
            GameSaveManager.Instance.LoadGame();

            // 로드가 한 번 완료된 후, 다른 문을 통과할 때 또 로드되지 않게 하려면
            // 아래 주석을 해제하여 플래그를 꺼줄 수 있습니다.
            // GameSaveManager.Instance.isContinueGame = false;
        }
    }
}