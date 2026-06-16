using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeManage: MonoBehaviour
{
    [Header("시간 설정")]
    public float totalCycleTime; // 전체 주기 (2분)
    [Range(0, 1)]
    public float currentTime;

    [Header("광원 설정")]
    public Light2D sunLight;
    public float maxIntensity; // 낮일 때 밝기
    public float minIntensity; // 밤일 때 밝기

    [Header("시네머신 v3 카메라 설정")]
    public CinemachineCamera virtualCamera;
    public float dayLensSize;       // 낮 렌즈 크기
    public float nightLensSize;    // 밤 렌즈 크기 (확대, 숫자가 작을수록 화면 확대)
    public float zoomSpeed;         // 확대/축소 속도

    public static TimeManage instance;

    void Awake()
    {
        // 중요: 이 코드가 있어야 외부에서 TimeManage.instance 로 접근이 가능
        if (instance == null) instance = this;
        if (virtualCamera == null) virtualCamera = GetComponent<CinemachineCamera>();
    }
    void Start()
    {
        // ★ 씬이 시작되면 DontDestroyOnLoad로 넘어온 플레이어를 태그로 찾습니다.
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null && virtualCamera != null)
        {
            // 시네머신 v3 규칙에 맞게 Follow와 LookAt 타겟을 넘어온 플레이어로 강제 지정합니다.
            virtualCamera.Follow = player.transform;

            // 만약 2D 게임이라 LookAt은 필요 없다면 아래 줄은 주석 처리하거나 지우셔도 됩니다.
            virtualCamera.LookAt = player.transform;

            Debug.Log("시네머신 카메라가 넘어온 플레이어를 성공적으로 트래킹하기 시작했습니다.");
        }
        else
        {
            Debug.LogWarning("플레이어 또는 시네머신 가상 카메라는 찾지 못했습니다.");
        }
    }

    void Update()
    {
        // 1. 시간 흐름 업데이트
        currentTime += Time.deltaTime / totalCycleTime;
        if (currentTime >= 1f) currentTime = 0f;

        float targetLensSize;

        if (currentTime <= 0.7f)
        {
            sunLight.intensity = maxIntensity;

            targetLensSize = dayLensSize;
        }
        else if (currentTime > 0.5f && currentTime <= 0.7f)
        {
            // 0.5 ~ 0.7 사이의 현재 시간 비율을 0에서 1 사이로 정규화(계산)합니다.
            float t = (currentTime - 0.5f) / (0.7f - 0.5f);

            // 낮 밝기(max)에서 밤 밝기(min)로 t 비율에 맞춰 서서히 슬라이딩하듯 어둡게 만듭니다.
            sunLight.intensity = Mathf.Lerp(maxIntensity, minIntensity, t);

            // 카메라도 시간에 비례해서 서서히 목표치 렌즈 크기로 좁혀나갑니다.
            targetLensSize = Mathf.Lerp(dayLensSize, nightLensSize, t);
        }
        // [구간 3] 0.7 ~ 1.0 : 완전한 밤 상태 유지 (0.7이 되는 순간 완벽한 암흑 도달)
        else
        {
            sunLight.intensity = minIntensity;
            targetLensSize = nightLensSize;
        }
        if (virtualCamera != null)
        {
            // 구조체 데이터를 변수에 임시 저장 (v3 규칙)
            LensSettings currentLens = virtualCamera.Lens;

            if (currentLens.Orthographic)
            {
                // 2D 모드일 때 (Orthographic Size 조절)
                currentLens.OrthographicSize = Mathf.Lerp(currentLens.OrthographicSize, targetLensSize, Time.deltaTime * zoomSpeed);
            }

            // 변경된 렌즈 구조체 값을 카메라에 다시 적용
            virtualCamera.Lens = currentLens;
        }
    }
}
