using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlacementRangeIndicator : MonoBehaviour
{
    public static PlacementRangeIndicator Instance { get; private set; }

    private LineRenderer lineRenderer;
    [SerializeField] private int segments = 64;
    private float currentRadius;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;

        // 부모-자식 분리형(방법2)이면 gameObject.SetActive(false); 로 쓰셔도 됩니다.
        lineRenderer.enabled = false;
    }

    // ★ PlacementManager가 아이템의 MaxPlacementDistance를 이쪽(radius)으로 넘겨줍니다.
    public void Show(Vector3 center, float radius)
    {
        currentRadius = radius; // 받아온 반지름 저장 (아이템별 사거리 동적 조정)

        // 방법 1 기준 컴포넌트 켜기 (방법 2라면 gameObject.SetActive(true);)
        lineRenderer.enabled = true;

        DrawCircle(center, currentRadius);
    }

    public void UpdatePosition(Vector3 center)
    {
        if (lineRenderer != null && lineRenderer.enabled)
        {
            DrawCircle(center, currentRadius);
        }
    }

    public void Hide()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // 방법 2라면 gameObject.SetActive(false);
        }
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            // ★ 여기서 입력받은 동적 radius를 곱해주기 때문에 원의 크기가 조절됩니다.
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, y, 0f) + center;
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}