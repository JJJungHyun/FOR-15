using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class Tooltip : MonoBehaviour
{
    [Header("Tooltip Settings")]
    [SerializeField] private Vector2 offset = new Vector2(15f, 15f);
    [SerializeField] private float padding = 25f;

    protected RectTransform rectTransform;

    private const float moveThreshold = 1.0f;
    private static Vector3 lastMousePos = new Vector3(-1, -1, -1);
    private static bool canShowAfterMove = false;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public static void ResetTooltipState()
    {
        lastMousePos = Input.mousePosition;
        canShowAfterMove = false;
    }

    public virtual void ShowTooltip()
    {
        if (!canShowAfterMove)
        {
            float dist = Vector3.Distance(Input.mousePosition, lastMousePos);
            if (dist > moveThreshold)
            {
                canShowAfterMove = true;
            }
            else
            {
                return;
            }
        }

        lastMousePos = Input.mousePosition;
        gameObject.SetActive(true);
    }

    public virtual void HideTooltip()
    {
        gameObject.SetActive(false);
    }
    protected virtual void Update()
    {
        if (gameObject.activeSelf) FollowMouse();
    }

    private void FollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 targetPos = mousePos + offset;

        Vector2 pivot = Vector2.zero;
        if (targetPos.x + rectTransform.rect.width > Screen.width - padding) pivot.x = 1f;
        if (targetPos.y + rectTransform.rect.height > Screen.height - padding) pivot.y = 1f;

        rectTransform.pivot = pivot;
        rectTransform.position = targetPos;
    }

    public static Vector3 ResetMousePosition() => lastMousePos = new Vector3(-1, -1, -1);
}