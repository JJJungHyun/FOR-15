using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal; // 2D Light 사용 시 필요

public class TimeManagerTick : MonoBehaviour
{
    public static TimeManagerTick Instance;

    [Header("틱 설정 (1분 낮, 1분 밤)")]
    public int ticksPerSecond = 20;     // 1초당 틱 횟수
    public int dayTicks = 1200;         // 20틱 * 60초 = 1200틱
    public int nightTicks = 1200;       // 20틱 * 60초 = 1200틱

    [Header("현재 상태")]
    public int currentTick = 0;
    public bool isPaused = false;       // 타이틀 화면 여부
    public bool isLoading = false;      // 로딩 화면 여부

    [Header("2D 조명 연출")]
    public Light2D globalLight;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.15f, 0.15f, 0.35f); // 어두운 밤 색상

    private float tickTimer = 0f;
    private int totalCycleTicks;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            totalCycleTicks = dayTicks + nightTicks;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 타이틀이거나 로딩 중이면 틱을 계산하지 않음
        if (isPaused || isLoading) return;

        tickTimer += Time.deltaTime;
        float secondsPerTick = 1f / ticksPerSecond;

        while (tickTimer >= secondsPerTick)
        {
            OnTick();
            tickTimer -= secondsPerTick;
        }

        // 시각 효과는 매 프레임 부드럽게 업데이트
        UpdateVisuals();
    }

    void OnTick()
    {
        currentTick++;
        if (currentTick >= totalCycleTicks) currentTick = 0;

        // 예시: 100틱마다 실행될 로직이 있다면 여기에 추가
        // if (currentTick % 100 == 0) { ... }
    }

    void UpdateVisuals()
    {
        if (globalLight == null) return;

        float t;
        if (currentTick < dayTicks) // 낮 시간
        {
            t = (float)currentTick / dayTicks;
            // 낮에서 밤으로 서서히 전환 (Lerp)
            globalLight.color = Color.Lerp(dayColor, nightColor, t);
        }
        else // 밤 시간
        {
            t = (float)(currentTick - dayTicks) / nightTicks;
            // 밤에서 낮으로 서서히 전환
            globalLight.color = Color.Lerp(nightColor, dayColor, t);
        }
    }

    public void StartLoading() => isLoading = true;
    public void FinishLoading() => isLoading = false;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름이 "Title"이면 일시정지
        isPaused = (scene.name == "Title");

        // 새로운 씬의 조명을 찾아 연결
        if (globalLight == null)
        {
            GameObject lightObj = GameObject.Find("Global Light 2D");
            if (lightObj != null) globalLight = lightObj.GetComponent<Light2D>();
        }
    }

    // 현재 낮인지 밤인지 확인하는 헬퍼 함수
    public bool IsDay() => currentTick < dayTicks;
}