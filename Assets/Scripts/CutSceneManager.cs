using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance { get; private set; }

    [System.Serializable]
    public struct CutscenePage
    {
        public Sprite p_Image;

        [TextArea(3, 5)]
        public string dialogueText;
    }

    [System.Serializable]
    public struct CutsceneData
    {
        public string cutsceneName;
        public List<CutscenePage> pages;
    }

    [Header("UI References")]
    [SerializeField] private Image cutsceneImageTarget;
    [SerializeField] private TextMeshProUGUI cutsceneTextTarget;
    [SerializeField] private GameObject cutsceneCanvas;

    [Header("Cutscene Databases")]
    [SerializeField] private List<CutsceneData> cutsceneDatabase;

    private List<CutscenePage> currentPlayingPages;
    private int currentIndex;
    private bool isCutsceneActive;
    private bool isPrimaryInstance;
    private System.Action onCutsceneComplete;

    public bool IsCutsceneActive => isCutsceneActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            isPrimaryInstance = false;
            Destroy(gameObject);
            return;
        }

        Instance = this;
        isPrimaryInstance = true;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
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
        if (!isPrimaryInstance)
        {
            return;
        }

        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isCutsceneActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            NextImage();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUIElementsInActiveScene();

        if (scene.name == "Beach")
        {
            StartCoroutine(DelayedStartCutscene("Beach", 0.1f));
        }
        else if (scene.name == "Forest")
        {
            StartCoroutine(DelayedStartCutscene("Forest", 0.1f));
        }
    }

    private void FindUIElementsInActiveScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (!currentScene.isLoaded)
        {
            return;
        }
        foreach (GameObject rootObject in currentScene.GetRootGameObjects())
        {
            Transform[] children = rootObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == "CutsceneCanvas")
                {
                    cutsceneCanvas = child.gameObject;
                }
                else if (child.name == "CutsceneImage")
                {
                    cutsceneImageTarget = child.GetComponent<Image>();
                }
                else if (child.name == "CutsceneText")
                {
                    cutsceneTextTarget = child.GetComponent<TextMeshProUGUI>();
                }
            }
        }
    }

    private IEnumerator DelayedStartCutscene(string name, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        StartCutscene(name);
    }

    public void StartCutscene(string name)
    {
        StartCutscene(name, (System.Action)null);
    }

    public bool StartCutscene(string name, string nextSceneName)
    {
        return StartCutscene(name, () =>
        {
            if (!string.IsNullOrWhiteSpace(nextSceneName))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(nextSceneName);
            }
        });
    }

    public bool StartCutscene(string name, System.Action onComplete)
    {
        if (cutsceneDatabase == null)
        {
            Debug.LogError($"'{name}' cutscene database is missing.");
            return false;
        }

        CutsceneData targetData = cutsceneDatabase.Find(data => data.cutsceneName == name);
        if (string.IsNullOrEmpty(targetData.cutsceneName) || targetData.pages == null || targetData.pages.Count == 0)
        {
            Debug.LogError($"'{name}' cutscene data was not found or has no pages.");
            return false;
        }

        if (cutsceneCanvas == null || cutsceneImageTarget == null)
        {
            FindUIElementsInActiveScene();
        }

        if (cutsceneCanvas == null || cutsceneImageTarget == null)
        {
            Debug.LogWarning($"'{name}' cutscene UI was not found.");
            return false;
        }

        currentPlayingPages = targetData.pages;
        currentIndex = 0;
        isCutsceneActive = true;
        onCutsceneComplete = onComplete;

        cutsceneCanvas.SetActive(true);
        UpdateCutsceneDisplay();

        Debug.Log($"Cutscene started: {name}");
        return true;
    }

    private void NextImage()
    {
        currentIndex++;

        if (currentIndex >= currentPlayingPages.Count)
        {
            EndCutscene();
        }
        else
        {
            UpdateCutsceneDisplay();
        }
    }

    private void UpdateCutsceneDisplay()
    {
        if (currentPlayingPages == null || currentIndex >= currentPlayingPages.Count)
        {
            return;
        }

        CutscenePage page = currentPlayingPages[currentIndex];

        if (cutsceneImageTarget != null)
        {
            cutsceneImageTarget.sprite = page.p_Image;
        }

        if (cutsceneTextTarget != null)
        {
            cutsceneTextTarget.text = page.dialogueText;
        }
    }

    private void EndCutscene()
    {
        isCutsceneActive = false;

        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(false);
        }

        currentPlayingPages = null;

        System.Action completeAction = onCutsceneComplete;
        onCutsceneComplete = null;
        completeAction?.Invoke();

        Debug.Log("Cutscene ended.");
    }
}