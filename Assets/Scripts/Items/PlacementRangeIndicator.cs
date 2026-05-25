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

        lineRenderer.enabled = false;
    }

    public void Show(Vector3 center, float radius)
    {
        currentRadius = radius; 

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
            lineRenderer.enabled = false; 
        }
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, y, 0f) + center;
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}