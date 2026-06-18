#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public static OptionManager Instance { get; private set; }

    [Header("Option UI")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer mainMixer;

    private Transform persistentCanvasTransform;
    private bool isPrimaryInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            isPrimaryInstance = false;
            return;
        }

        Instance = this;
        isPrimaryInstance = true;
        DontDestroyOnLoad(gameObject);
        MoveOptionPanelToPersistentCanvas();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (persistentCanvasTransform != null)
        {
            Destroy(persistentCanvasTransform.gameObject);
        }
    }

    private void OnEnable()
    {
        if (isPrimaryInstance)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDisable()
    {
        if (isPrimaryInstance)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        if (!isPrimaryInstance) return;

        BindMissingReferences();
        InitializeSliders();
        CloseOption();
    }

    private void Update()
    {
        if (!isPrimaryInstance) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOption();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isPrimaryInstance) return;

        BindMissingReferences();
        InitializeSliders();
    }

    private void BindMissingReferences()
    {
        if (optionPanel == null)
        {
            UIOptionPanel uiOptionPanel = FindFirstObjectByType<UIOptionPanel>(FindObjectsInactive.Include);
            if (uiOptionPanel != null)
            {
                optionPanel = uiOptionPanel.gameObject;
            }
        }

        if (optionPanel == null)
        {
            optionPanel = GameObject.Find("Option Panel");
        }

        if (optionPanel == null)
        {
            optionPanel = FindInactiveOptionPanelByName();
        }

        if (optionPanel == null) return;

        MoveOptionPanelToPersistentCanvas();

        if (musicSlider == null)
        {
            musicSlider = FindSliderInOptionPanel("BGMSlider");
        }

        if (sfxSlider == null)
        {
            sfxSlider = FindSliderInOptionPanel("SFXSlider");
        }
    }

    private Slider FindSliderInOptionPanel(string sliderName)
    {
        Slider[] sliders = optionPanel.GetComponentsInChildren<Slider>(true);
        foreach (Slider slider in sliders)
        {
            if (slider.name == sliderName)
            {
                return slider;
            }
        }

        return null;
    }

    private GameObject FindInactiveOptionPanelByName()
    {
        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform target in transforms)
        {
            if (target != null && target.name == "Option Panel")
            {
                return target.gameObject;
            }
        }
        return null;
    }

    private void MoveOptionPanelToPersistentCanvas()
    {
        if (optionPanel == null) return;

        Transform persistentCanvas = GetOrCreatePersistentCanvas();
        if (optionPanel.transform.parent != persistentCanvas)
        {
            optionPanel.transform.SetParent(persistentCanvas, false);
        }

        optionPanel.transform.SetAsLastSibling();

        RectTransform panelRect = optionPanel.transform as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = Vector2.zero;
        }
    }

    private Transform GetOrCreatePersistentCanvas()
    {
        if (persistentCanvasTransform != null)
        {
            return persistentCanvasTransform;
        }

        GameObject canvasObject = GameObject.Find("Persistent Option Canvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Persistent Option Canvas");

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
        }

        DontDestroyOnLoad(canvasObject);
        persistentCanvasTransform = canvasObject.transform;
        return persistentCanvasTransform;
    }

    private void InitializeSliders()
    {
        InitializeSlider(musicSlider, "BGMVol");
        InitializeSlider(sfxSlider, "SFXVol");
    }

    private void InitializeSlider(Slider slider, string mixerParameter)
    {
        if (slider == null) return;

        slider.minValue = 0.0001f;
        slider.maxValue = 1f;

        if (mainMixer != null && mainMixer.GetFloat(mixerParameter, out float currentDb))
        {
            slider.value = Mathf.Pow(10f, currentDb / 20f);
        }
    }

    public void ToggleOption()
    {
        if (TryDelegateToPrimary(manager => manager.ToggleOption())) return;

        if (optionPanel == null)
        {
            BindMissingReferences();
        }

        if (optionPanel == null) return;

        MoveOptionPanelToPersistentCanvas();
        optionPanel.SetActive(!optionPanel.activeSelf);
    }

    public void OpenOption()
    {
        if (TryDelegateToPrimary(manager => manager.OpenOption())) return;

        if (optionPanel == null)
        {
            BindMissingReferences();
        }

        if (optionPanel != null)
        {
            MoveOptionPanelToPersistentCanvas();
            optionPanel.SetActive(true);
        }
    }

    public void CloseOption()
    {
        if (TryDelegateToPrimary(manager => manager.CloseOption())) return;

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
    }

    public void SetBGMVolume(float sliderValue)
    {
        if (TryDelegateToPrimary(manager => manager.SetBGMVolume(sliderValue))) return;

        SetMixerVolume("BGMVol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        if (TryDelegateToPrimary(manager => manager.SetSFXVolume(sliderValue))) return;

        SetMixerVolume("SFXVol", sliderValue);
    }

    private void SetMixerVolume(string mixerParameter, float sliderValue)
    {
        if (mainMixer == null) return;

        float safeValue = Mathf.Max(sliderValue, 0.0001f);
        float dbValue = Mathf.Clamp(Mathf.Log10(safeValue) * 20f, -80f, 0f);

        mainMixer.SetFloat(mixerParameter, dbValue);
    }

    public void ExitToTitle()
    {
        if (TryDelegateToPrimary(manager => manager.ExitToTitle())) return;

        CloseOption();

        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SaveGame();
        }

        Time.timeScale = 1f;
        KeepOnLoad.DestroyPersistentPlayer();

        if (MySceneManager.Instance != null)
        {
            MySceneManager.Instance.ChangeScene("TitleScene");
        }
        else
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    public void QuitGame()
    {
        if (TryDelegateToPrimary(manager => manager.QuitGame())) return;
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool TryDelegateToPrimary(System.Action<OptionManager> action)
    {
        if (Instance == null || Instance == this)
        {
            return false;
        }

        action?.Invoke(Instance);
        return true;
    }
}