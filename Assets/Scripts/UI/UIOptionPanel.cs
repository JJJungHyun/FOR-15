using UnityEngine;
using DG.Tweening;

public class UIOptionPanel : MonoBehaviour
{
    // 어디서나 접근할 수 있는 싱글톤 인스턴스
    public static UIOptionPanel Instance { get; private set; }

    [Header("UI References")]
    public RectTransform panelRect; // 움직일 패널

    [Header("Settings")]
    public float animationDuration = 0.5f;
    public Ease easeType = Ease.OutExpo;

    private float hiddenYPosition;
    private float visibleYPosition = 0f;
    private bool isOpen = false;

    private void Awake()
    {
        // --- [싱글톤 & 씬 전환 시 유지 코드] ---
        if (Instance == null)
        {
            Instance = this;
            // 이 오브젝트는 씬이 바뀌어도 파괴되지 않습니다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 다른 씬에서 이미 매니저가 있는 상태로 또 생성되었다면 파괴합니다.
            Destroy(gameObject);
            return;
        }
        // --------------------------------------

        hiddenYPosition = -Screen.height;

        if (panelRect != null)
        {
            panelRect.anchoredPosition = new Vector2(0, hiddenYPosition);
            panelRect.gameObject.SetActive(false);
            isOpen = false;
        }
    }

    private void Update()
    {
        // ESC 키 입력 체크
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (isOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        if (panelRect == null) return;

        isOpen = true;
        panelRect.gameObject.SetActive(true);
        panelRect.DOKill();
        panelRect.anchoredPosition = new Vector2(0, hiddenYPosition);

        panelRect.DOAnchorPosY(visibleYPosition, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true);
    }

    public void ClosePanel()
    {
        if (panelRect == null) return;

        isOpen = false;
        panelRect.DOKill();
        panelRect.DOAnchorPosY(hiddenYPosition, animationDuration)
            .SetEase(easeType)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                panelRect.gameObject.SetActive(false);
            });
    }

    public void OnQuitButtonClicked()
    {
        // 단순히 패널을 닫는 기능을 실행합니다.
        ClosePanel();

        // 만약 '게임 종료' 기능까지 넣고 싶다면 아래 주석을 해제하세요.
        // Application.Quit(); 
    }

    public void ExitToTitle()
    {
        // 1. 나가기 직전 자동 저장 실행
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SaveGame();
            Debug.Log("나가기 전 자동 저장 완료");
        }

        // 2. MySceneManager를 사용하여 타이틀 씬으로 이동 (애니메이션 재생됨)
        if (MySceneManager.Instance != null)
        {
            // "TitleScene" 부분에 실제 본인의 타이틀 씬 이름을 넣으세요.
            MySceneManager.Instance.ChangeScene("TitleScene");
        }
        else
        {
            // 만약 MySceneManager가 없다면 일반 방식으로라도 이동
            UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        }
    }
}