using UnityEditor;
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        BindMissingReferences();
        InitializeSliders();
        CloseOption();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOption();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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

    private void MoveOptionPanelToPersistentCanvas()
    {
        if (optionPanel == null) return;

        Transform persistentCanvas = GetOrCreatePersistentCanvas();
        if (optionPanel.transform.parent != persistentCanvas)
        {
            optionPanel.transform.SetParent(persistentCanvas, false);
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
        if (optionPanel == null)
        {
            BindMissingReferences();
        }

        if (optionPanel == null) return;

        optionPanel.SetActive(!optionPanel.activeSelf);
    }

    public void OpenOption()
    {
        if (optionPanel == null)
        {
            BindMissingReferences();
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
        }
    }

    public void CloseOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
    }

    public void SetBGMVolume(float sliderValue)
    {
        SetMixerVolume("BGMVol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
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
        CloseOption();

        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SaveGame();
        }

        Time.timeScale = 1f;
        KeepOnLoad.DestroyPersistentPlayer();

        QuitGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}