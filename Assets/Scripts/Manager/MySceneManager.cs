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
    public float wipeDuration = 0.5f;
    public float wipeDelay = 0.1f;
    public float textFadeDuration = 1.5f;
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

        // ==========================================
        // [추가] 1. 로딩 시작 시 틱(시간) 중지
        // ==========================================
        if (TimeManagerTick.Instance != null)
            TimeManagerTick.Instance.StartLoading();

        if (loadingText != null)
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1f);

        // --- 1단계: 화면 가리기 애니메이션 ---
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

        // --- 2단계: 비동기 로딩 (여기에 op가 있습니다!) ---
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName); // <-- op 선언
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
        op.allowSceneActivation = true; // 실제 씬 전환 승인

        while (!op.isDone) yield return null; // 씬 전환 완료될 때까지 대기

        // --- 3단계: 화면 드러내기 애니메이션 ---
        yield return new WaitForSecondsRealtime(0.3f);

        float outPos = -(Screen.width + 100);
        for (int i = 0; i < wipeBars.Length; i++)
        {
            if (wipeBars[i] == null) continue;
            wipeBars[i].DOAnchorPosX(outPos, wipeDuration).SetEase(wipeEase).SetDelay(i * wipeDelay).SetUpdate(true);
        }

        if (loadingText != null)
        {
            loadingText.DOFade(0, textFadeDuration).SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(wipeDuration + (wipeBars.Length * wipeDelay));

        // 3-3. 마지막 전체 그룹 페이드 아웃 및 종료
        if (fadeGroup != null)
        {
            fadeGroup.DOFade(0, 0.5f).SetUpdate(true).OnComplete(() => {
                fadeGroup.blocksRaycasts = false;
                isTransitioning = false;

                // ==========================================
                // [추가] 2. 로딩 화면이 완전히 사라지면 틱(시간) 재개
                // ==========================================
                if (TimeManagerTick.Instance != null)
                    TimeManagerTick.Instance.FinishLoading();
            });
        }
    }
}