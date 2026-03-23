using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using TMPro;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance;

    [Header("UI 연결")]
    public CanvasGroup fadeGroup;
    public RectTransform[] wipeBars;
    public TMP_Text loadingText;

    [Header("설정")]
    public float wipeDuration = 0.5f;   // 바가 움직이는 시간
    public float wipeDelay = 0.1f;      // 바 사이의 간격
    public float textFadeDuration = 1.5f; // ★텍스트가 서서히 사라지는 시간 (이 숫자를 조절하세요)
    public Ease wipeEase = Ease.InOutQuart;

    private bool isTransitioning = false;

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

        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0;
            fadeGroup.blocksRaycasts = false;
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (isTransitioning) return;

        if (fadeGroup == null || wipeBars == null || wipeBars.Length == 0)
        {
            Debug.LogError("MySceneManager: UI 요소가 연결되지 않았습니다!");
            return;
        }

        StartCoroutine(ProcessSceneTransition(sceneName));
    }

    private IEnumerator ProcessSceneTransition(string sceneName)
    {
        isTransitioning = true;
        fadeGroup.blocksRaycasts = true;

        // 시작 시 텍스트 알파값 초기화 (완전 불투명)
        if (loadingText != null) loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1f);

        // --- 1단계: 화면 가리기 ---
        fadeGroup.DOKill();
        fadeGroup.DOFade(1, 0.2f).SetUpdate(true);

        float screenWidth = Screen.width + 100;
        for (int i = 0; i < wipeBars.Length; i++)
        {
            if (wipeBars[i] == null) continue;
            wipeBars[i].DOKill();
            wipeBars[i].anchoredPosition = new Vector2(screenWidth, wipeBars[i].anchoredPosition.y);
            wipeBars[i].DOAnchorPosX(0, wipeDuration).SetEase(wipeEase).SetDelay(i * wipeDelay).SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(wipeDuration + (wipeBars.Length * wipeDelay));

        // --- 2단계: 비동기 로딩 ---
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (op.progress < 0.9f || timer < 1.0f)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            float displayProgress = Mathf.Min(timer * 100f, progress * 100f);

            if (loadingText != null)
                loadingText.text = $"Loading... {displayProgress:F0}%";

            yield return null;
        }

        if (loadingText != null) loadingText.text = "Loading... 100%";
        op.allowSceneActivation = true;

        while (!op.isDone) yield return null;

        // --- 3단계: 화면 드러내기 ---
        yield return new WaitForSecondsRealtime(0.3f);

        // 3-1. 배경 바(Bars)들이 나가는 애니메이션
        float outPos = -(Screen.width + 100);
        for (int i = 0; i < wipeBars.Length; i++)
        {
            if (wipeBars[i] == null) continue;
            wipeBars[i].DOAnchorPosX(outPos, wipeDuration).SetEase(wipeEase).SetDelay(i * wipeDelay).SetUpdate(true);
        }

        // 3-2. ★텍스트만 따로 서서히 사라지게 함 (독립적인 타이밍)
        if (loadingText != null)
        {
            loadingText.DOFade(0, textFadeDuration).SetUpdate(true);
        }

        // 바들이 다 나갈 때까지 기다림
        yield return new WaitForSecondsRealtime(wipeDuration + (wipeBars.Length * wipeDelay));

        // 3-3. 마지막 전체 그룹 페이드 아웃
        if (fadeGroup != null)
        {
            // 텍스트가 완전히 사라질 때까지 기다리거나, 배경을 먼저 치울 수 있습니다.
            // 여기서는 텍스트 페이드 시간에 맞춰 전체 그룹을 닫습니다.
            fadeGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => {
                fadeGroup.blocksRaycasts = false;
                isTransitioning = false;
            });
        }
    }
}