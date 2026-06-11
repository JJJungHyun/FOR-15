using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening; // DOTween 사용
using TMPro; // 텍스트메시프로 사용

public class GameOverManager : MonoBehaviour
{
    // 어디서든 호출할 수 있게 싱글톤으로 설정
    public static GameOverManager Instance;

    [Header("UI 연결")]
    [SerializeField] private CanvasGroup _gameOverCanvasGroup; // 화면을 어둡게 할 패널
    [SerializeField] private TMP_Text _gameOverText;         // "GAME OVER" 텍스트
    [SerializeField] private string _titleSceneName = "Title"; // 타이틀 씬 이름

    [Header("설정")]
    [SerializeField] private float _fadeDuration = 2.0f; // 어두워지는 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬이 바뀌어도 이 오브젝트(GameOverManager)를 파괴하지 마세요!
            DontDestroyOnLoad(gameObject);
        } 
    else
        {
            // 씬이 전환되면서 새로 생긴 중복 매니저가 있다면 파괴합니다.
            Destroy(gameObject);
            return;
        }

        // 처음엔 UI를 안 보이게 설정
        _gameOverCanvasGroup.alpha = 0;
        _gameOverCanvasGroup.blocksRaycasts = false;
        _gameOverText.gameObject.SetActive(false);
    }

    // 플레이어 체력이 0일 때 호출될 메인 함수
    public void StartGameOver()
    {
        // 중복 실행 방지
        _gameOverCanvasGroup.blocksRaycasts = true;

        // 1. 화면 점점 어둡게 (Fade In)
        _gameOverCanvasGroup.DOFade(1f, _fadeDuration).OnComplete(() =>
        {
            // 2. 어두워진 후 게임 오버 텍스트 활성화 및 연출
            ShowGameOverText();
        });
    }

    private void ShowGameOverText()
    {
        _gameOverText.gameObject.SetActive(true);
        // 텍스트가 살짝 커지면서 나타나는 연출
        _gameOverText.transform.localScale = Vector3.zero;
        _gameOverText.transform.DOScale(1.2f, 1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // 3. 2초 대기 후 타이틀 화면으로 이동
            Invoke("GoToTitle", 2f);
        });
    }

    private void GoToTitle()
    {
        SceneManager.LoadScene(_titleSceneName);
    }
}