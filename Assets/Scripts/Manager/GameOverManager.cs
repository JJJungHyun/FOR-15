using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI References")]
    [SerializeField] private CanvasGroup _gameOverCanvasGroup;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private string _titleSceneName = "TitleScene";

    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 2.0f;

    private bool _isGameOverRunning;
    private Coroutine _returnToTitleCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ResolveReferences();
        HideGameOverUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isGameOverRunning = false;
        ResolveReferences();
        HideGameOverUI();
    }

    public void StartGameOver()
    {
        if (_isGameOverRunning) return;

        _isGameOverRunning = true;
        ResolveReferences();

        if (_gameOverCanvasGroup == null || _gameOverText == null)
        {
            Debug.LogWarning("GameOver UI references were not found. Loading the title scene directly.");
            GoToTitle();
            return;
        }

        if (_returnToTitleCoroutine != null)
        {
            StopCoroutine(_returnToTitleCoroutine);
            _returnToTitleCoroutine = null;
        }

        _gameOverCanvasGroup.DOKill();
        _gameOverText.transform.DOKill();

        _gameOverCanvasGroup.gameObject.SetActive(true);
        _gameOverCanvasGroup.transform.localScale = Vector3.one;
        _gameOverCanvasGroup.alpha = 0f;
        _gameOverCanvasGroup.interactable = true;
        _gameOverCanvasGroup.blocksRaycasts = true;

        _gameOverText.gameObject.SetActive(false);

        _gameOverCanvasGroup
            .DOFade(1f, _fadeDuration)
            .SetUpdate(true)
            .OnComplete(ShowGameOverText);
    }

    private void ShowGameOverText()
    {
        if (_gameOverText == null)
        {
            GoToTitle();
            return;
        }

        _gameOverText.gameObject.SetActive(true);
        _gameOverText.transform.localScale = Vector3.zero;
        _gameOverText.transform
            .DOScale(1.2f, 1f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() => _returnToTitleCoroutine = StartCoroutine(ReturnToTitleAfterDelay()));
    }

    private IEnumerator ReturnToTitleAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        GoToTitle();
    }

    private void GoToTitle()
    {
        Time.timeScale = 1f;
        string targetScene = string.IsNullOrWhiteSpace(_titleSceneName) ? "TitleScene" : _titleSceneName;
        SceneManager.LoadScene(targetScene);
    }

    private void ResolveReferences()
    {
        if (_gameOverCanvasGroup == null)
        {
            GameObject canvasObject = GameObject.Find("GameOverCanvas");
            if (canvasObject != null)
            {
                _gameOverCanvasGroup = canvasObject.GetComponent<CanvasGroup>();
            }
        }

        if (_gameOverText == null)
        {
            GameObject textObject = GameObject.Find("GameOverText");
            if (textObject != null)
            {
                _gameOverText = textObject.GetComponent<TMP_Text>();
            }
        }
    }

    private void HideGameOverUI()
    {
        if (_gameOverCanvasGroup != null)
        {
            _gameOverCanvasGroup.DOKill();
            _gameOverCanvasGroup.transform.localScale = Vector3.one;
            _gameOverCanvasGroup.alpha = 0f;
            _gameOverCanvasGroup.interactable = false;
            _gameOverCanvasGroup.blocksRaycasts = false;
        }

        if (_gameOverText != null)
        {
            _gameOverText.transform.DOKill();
            _gameOverText.gameObject.SetActive(false);
            _gameOverText.transform.localScale = Vector3.one;
        }
    }
}