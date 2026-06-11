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

    public Light playerLight;

    public static TimeManage instance;

    void Update()
    {
        // 1. 시간 흐름 업데이트
        currentTime += Time.deltaTime / totalCycleTime;
        if (currentTime >= 1f) currentTime = 0f;

        if (currentTime <= 0.5f)
        {
            sunLight.intensity = Mathf.Clamp(Mathf.Sin(currentTime * Mathf.PI * 2) * maxIntensity, minIntensity, maxIntensity);

            if (playerLight != null) playerLight.enabled = false;
        }
        else
        {
            // 밤: 최소 밝기 유지
            sunLight.intensity = minIntensity;

            if (playerLight != null) playerLight.enabled = true;
        }
    }
    void Awake()
    {
        // 중요: 이 코드가 있어야 외부에서 TimeManage.instance 로 접근이 가능합니다.
        if (instance == null) instance = this;
    }
}
