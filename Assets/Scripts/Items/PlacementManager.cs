using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [Header("Preview Colors")]
    [SerializeField] private Color possibleColor = new Color(0f, 0.5f, 1f, 0.4f); // 연한 파란색
    [SerializeField] private Color impossibleColor = new Color(1f, 0f, 0f, 0.4f); // 연한 빨간색

    [Header("Layers for Collision Check")]
    [SerializeField] private LayerMask obstacleLayers;

    private DeployableItem currentItem;
    private Transform playerTransform;
    private GameObject previewInstance;
    private SpriteRenderer previewRenderer;

    private bool hasCollider = false;
    private bool isBoxCollider = false;
    private Vector2 colliderSizeOrRadius;

    private bool isPlacementMode = false;
    private bool isValidPosition = false;
    private Camera mainCamera;

    private void Awake()
    {
        EnsureCameraReference();
    }

    private void Update()
    {
        if (!isPlacementMode || previewInstance == null) return;

        UpdatePreviewPosition();
        CheckPlacementValidity();
        HandleInput();
    }

    private void EnsureCameraReference()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
        }
    }

    public void StartPlacementMode(DeployableItem item, Transform player)
    {
        CancelPlacementMode();

        currentItem = item;
        playerTransform = player;
        isPlacementMode = true;

        EnsureCameraReference();

        previewInstance = new GameObject("PlacementPreview");
        previewRenderer = previewInstance.AddComponent<SpriteRenderer>();
        previewRenderer.sprite = item.Icon;
        previewRenderer.sortingOrder = 10;

        if (item.DeployablePrefab != null)
        {
            previewInstance.transform.localScale = item.DeployablePrefab.transform.localScale;

            Collider2D originalCollider = item.DeployablePrefab.GetComponentInChildren<Collider2D>();
            if (originalCollider != null)
            {
                hasCollider = true;
                if (originalCollider is BoxCollider2D box)
                {
                    isBoxCollider = true;
                    colliderSizeOrRadius = box.size;
                }
                else if (originalCollider is CircleCollider2D circle)
                {
                    isBoxCollider = false;
                    colliderSizeOrRadius = new Vector2(circle.radius, 0);
                }
            }
        }

        PlacementRangeIndicator.Instance?.Show(playerTransform.position, item.MaxPlacementDistance);
    }

    private void UpdatePreviewPosition()
    {
        EnsureCameraReference();
        if (mainCamera == null || previewInstance == null) return;

        Plane xyPlane = new Plane(Vector3.forward, Vector3.zero);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (xyPlane.Raycast(ray, out float enter))
        {
            Vector3 mouseWorldPos = ray.GetPoint(enter);
            mouseWorldPos.z = 0f; 

            previewInstance.transform.position = mouseWorldPos;
        }

        if (PlacementRangeIndicator.Instance != null)
        {
            PlacementRangeIndicator.Instance.UpdatePosition(playerTransform.position);
        }
    }

    private void CheckPlacementValidity()
    {
        if (previewInstance == null || currentItem == null) return;

        Vector2 currentPos = previewInstance.transform.position;
        float distance = Vector2.Distance(playerTransform.position, currentPos);

        if (distance > currentItem.MaxPlacementDistance)
        {
            SetPreviewState(false);
            return;
        }

        bool isOverlapping = false;
        if (hasCollider)
        {
            Vector2 finalScale = previewInstance.transform.localScale;

            if (isBoxCollider)
            {
                Vector2 boxSize = new Vector2(colliderSizeOrRadius.x * finalScale.x, colliderSizeOrRadius.y * finalScale.y);
                isOverlapping = Physics2D.OverlapBox(currentPos, boxSize, 0f, obstacleLayers);
            }
            else
            {
                float radius = colliderSizeOrRadius.x * Mathf.Max(finalScale.x, finalScale.y);
                isOverlapping = Physics2D.OverlapCircle(currentPos, radius, obstacleLayers);
            }
        }
        else
        {
            isOverlapping = Physics2D.OverlapPoint(currentPos, obstacleLayers);
        }

        SetPreviewState(!isOverlapping);
    }

    private void SetPreviewState(bool possible)
    {
        isValidPosition = possible;
        if (previewRenderer != null)
        {
            previewRenderer.color = possible ? possibleColor : impossibleColor;
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isValidPosition) Deploy();
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacementMode();
        }
    }

    private void Deploy()
    {
        if (currentItem == null || previewInstance == null) return;

        Instantiate(currentItem.DeployablePrefab, previewInstance.transform.position, Quaternion.identity);

        var invController = FindFirstObjectByType<InventoryController>();
        if (invController != null)
        {
            invController.GetInventory().RemoveItem(currentItem);
        }

        CancelPlacementMode();
    }

    public void CancelPlacementMode()
    {
        if (previewInstance != null) Destroy(previewInstance);
        currentItem = null;
        isPlacementMode = false;
        isValidPosition = false;
        hasCollider = false;

        PlacementRangeIndicator.Instance?.Hide();
    }
}