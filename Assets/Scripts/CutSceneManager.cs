using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    // --- 싱글톤 (Singleton) 구현 ---
    public static CutSceneManager Instance { get; private set; }

    [System.Serializable]
    public struct CutsceneData
    {
        public string cutsceneName;       // 컷씬을 식별할 이름 (예: "Opening", "BossClear")
        public List<Sprite> p_Images;     // 이 컷씬에 들어갈 PNG 이미지 리스트
    }

    [Header("UI References")]
    [SerializeField] private Image cutsceneImageTarget; // UI의 Image 컴포넌트
    [SerializeField] private GameObject cutsceneCanvas; // 컷씬 캔버스 오브젝트

    [Header("Cutscene Databases")]
    [SerializeField] private List<CutsceneData> cutsceneDatabase; // 여러 개의 컷씬 리스트를 관리

    private List<Sprite> currentPlayingImages; // 현재 재생 중인 컷씬의 이미지 리스트
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
        // 유니티 시스템에 씬이 로드되었을 때 OnSceneLoaded 함수를 실행하라고 등록합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위해 이벤트 연결을 해제합니다.
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
        // 만약 새로 로드된 씬 이름이 정확히 "Beach" 라면
        if (scene.name == "Beach")
        {
            // 약간의 딜레이를 주어 씬 로딩 직후 화면이 안정화되면 컷씬을 틀어줍니다.
            StartCoroutine(DelayedStartCutscene("Beach", 0.1f));
        }
        else if (scene.name == "Forest")
        {
            // 약간의 딜레이를 주어 씬 로딩 직후 화면이 안정화되면 컷씬을 틀어줍니다.
            StartCoroutine(DelayedStartCutscene("Forest", 0.1f));
        }
    }

    private System.Collections.IEnumerator DelayedStartCutscene(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCutscene(name);
    }

    // --- 외부에서 컷씬을 시작할 때 호출하는 함수 (조건이 맞을 때 사용) ---
    public void StartCutscene(string name)
    {
        // 데이터베이스에서 이름이 일치하는 컷씬 찾기
        CutsceneData targetData = cutsceneDatabase.Find(data => data.cutsceneName == name);

        // 예외 처리: 없는 이름을 불렀을 때
        if (string.IsNullOrEmpty(targetData.cutsceneName) || targetData.p_Images == null || targetData.p_Images.Count == 0)
        {
            Debug.LogError($"'{name}' 이름의 컷씬 데이터를 찾을 수 없거나 이미지가 없습니다!");
            return;
        }

        // 재생할 이미지 리스트 세팅 및 초기화
        currentPlayingImages = targetData.p_Images;
        currentIndex = 0;
        isCutsceneActive = true;

        cutsceneCanvas.SetActive(true);
        UpdateCutsceneDisplay();

        Debug.Log($"컷씬 시작: {name}");
        // 여기에 플레이어 조작을 멈추는 코드(예: PlayerController 끄기)를 넣으면 좋습니다.
    }

    private void NextImage()
    {
        currentIndex++;

        // 현재 재생 중인 리스트의 이미지를 다 보았으면 종료
        if (currentIndex >= currentPlayingImages.Count)
            EndCutscene();
        else
            UpdateCutsceneDisplay();
    }

    private void UpdateCutsceneDisplay()
    {
        if (currentPlayingImages != null && currentIndex < currentPlayingImages.Count)
        {
            cutsceneImageTarget.sprite = currentPlayingImages[currentIndex];
        }
    }

    private void EndCutscene()
    {
        isCutsceneActive = false;
        cutsceneCanvas.SetActive(false);
        currentPlayingImages = null;

        Debug.Log("컷씬 종료! 게임으로 돌아갑니다.");
        // 여기에 플레이어 조작을 다시 켜는 코드를 넣으면 됩니다.
    }
}