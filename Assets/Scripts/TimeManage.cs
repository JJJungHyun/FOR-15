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

    public Light playerLight;

    public static TimeManage instance;
    void Awake()
    {
        // 중요: 이 코드가 있어야 외부에서 TimeManage.instance 로 접근이 가능
        if (instance == null) instance = this;
        if (virtualCamera == null) virtualCamera = GetComponent<CinemachineCamera>();
    }
    void Update()
    {
        // 1. 시간 흐름 업데이트
        currentTime += Time.deltaTime / totalCycleTime;
        if (currentTime >= 1f) currentTime = 0f;

        float targetLensSize;

        if (currentTime <= 0.5f)
        {
            sunLight.intensity = Mathf.Clamp(Mathf.Sin(currentTime * Mathf.PI * 2) * maxIntensity, minIntensity, maxIntensity);
            if (playerLight != null) playerLight.enabled = false;

            targetLensSize = dayLensSize;
        }
        else
        {
            // 밤: 최소 밝기 유지
            sunLight.intensity = minIntensity;
            if (playerLight != null) playerLight.enabled = true;
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
