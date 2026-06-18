using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // ★ TextMeshPro 사용을 위해 추가!

public class CutSceneManager : MonoBehaviour
{
    // --- 싱글톤 (Singleton) 구현 ---
    public static CutSceneManager Instance { get; private set; }

    // ★ 대사 텍스트를 이미지와 1:1로 매칭시키기 위해 데이터 구조 수정
    [System.Serializable]
    public struct CutscenePage
    {
        public Sprite p_Image;       // 컷씬 한 장의 이미지
        [TextArea(3, 5)]
        public string dialogueText;  // 그 이미지에 매칭될 대사/자막
    }

    [System.Serializable]
    public struct CutsceneData
    {
        public string cutsceneName;           // 컷씬을 식별할 이름 (예: "Opening", "Beach")
        public List<CutscenePage> pages;      // 이미지와 대사가 세트로 묶인 페이지 리스트
    }

    [Header("UI References")]
    [SerializeField] private Image cutsceneImageTarget; // UI의 Image 컴포넌트
    [SerializeField] private TextMeshProUGUI cutsceneTextTarget; // ★ 추가: 대사가 출력될 TextMeshPro 컴포넌트
    [SerializeField] private GameObject cutsceneCanvas; // 컷씬 캔버스 오브젝트

    [Header("Cutscene Databases")]
    [SerializeField] private List<CutsceneData> cutsceneDatabase; // 여러 개의 컷씬 리스트를 관리

    private List<CutscenePage> currentPlayingPages; // 현재 재생 중인 컷씬의 페이지 리스트
    private int currentIndex = 0;
    private bool isCutsceneActive = false;

    private void Awake()
    {
        // 싱글톤 세팅
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
    }

    // --- 씬 로드 감지 이벤트 연결 ---
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
        // 시작할 때는 컷씬 캔버스를 꺼둡니다.
        if (cutsceneCanvas != null)
            cutsceneCanvas.SetActive(false);
    }

    private void Update()
    {
        // 컷씬이 활성화된 상태에서만 입력을 받음
        if (!isCutsceneActive) return;

        // 마우스 좌클릭 또는 스페이스바를 누르면 다음 이미지로
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            NextImage();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 아까 에러 현상을 해결하기 위해 추가한 실시간 씬 UI 재검색 함수 호출
        // (OpTionManager와 유사하게 씬 전환 시 자동으로 UI 연결 유실을 방지합니다)
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

    // 씬이 넘어왔을 때 캔버스 및 텍스트 오브젝트를 자동 추적하는 예외 방지 함수
    private void FindUIElementsInActiveScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (!currentScene.isLoaded) return;

        foreach (GameObject rootObj in currentScene.GetRootGameObjects())
        {
            Transform[] allChildren = rootObj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (child.name == "CutsceneCanvas") cutsceneCanvas = child.gameObject;
                else if (child.name == "CutsceneImage") cutsceneImageTarget = child.GetComponent<Image>();
                else if (child.name == "CutsceneText") cutsceneTextTarget = child.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private System.Collections.IEnumerator DelayedStartCutscene(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCutscene(name);
    }

    // --- 외부에서 컷씬을 시작할 때 호출하는 함수 ---
    public void StartCutscene(string name)
    {
        // 데이터베이스에서 이름이 일치하는 컷씬 찾기
        CutsceneData targetData = cutsceneDatabase.Find(data => data.cutsceneName == name);

        // 예외 처리
        if (string.IsNullOrEmpty(targetData.cutsceneName) || targetData.pages == null || targetData.pages.Count == 0)
        {
            Debug.LogError($"'{name}' 이름의 컷씬 데이터를 찾을 수 없거나 페이지가 없습니다!");
            return;
        }

        // 혹시라도 UI 컴포넌트 유실 시 실시간 복구
        if (cutsceneCanvas == null || cutsceneTextTarget == null) FindUIElementsInActiveScene();

        // 재생할 페이지 리스트 세팅 및 초기화
        currentPlayingPages = targetData.pages;
        currentIndex = 0;
        isCutsceneActive = true;

        if (cutsceneCanvas != null) cutsceneCanvas.SetActive(true);
        UpdateCutsceneDisplay();

        Debug.Log($"컷씬 시작: {name}");
    }

    private void NextImage()
    {
        currentIndex++;

        // 현재 재생 중인 리스트의 이미지를 다 보았으면 종료
        if (currentIndex >= currentPlayingPages.Count)
            EndCutscene();
        else
            UpdateCutsceneDisplay();
    }

    private void UpdateCutsceneDisplay()
    {
        if (currentPlayingPages != null && currentIndex < currentPlayingPages.Count)
        {
            // 이미지 출력
            if (cutsceneImageTarget != null)
                cutsceneImageTarget.sprite = currentPlayingPages[currentIndex].p_Image;

            // ★ 텍스트 출력
            if (cutsceneTextTarget != null)
                cutsceneTextTarget.text = currentPlayingPages[currentIndex].dialogueText;
        }
    }

    private void EndCutscene()
    {
        isCutsceneActive = false;
        if (cutsceneCanvas != null) cutsceneCanvas.SetActive(false);
        currentPlayingPages = null;

        Debug.Log("컷씬 종료! 게임으로 돌아갑니다.");
    }
}