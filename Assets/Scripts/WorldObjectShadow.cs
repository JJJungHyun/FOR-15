using UnityEngine;

[DisallowMultipleComponent]
public class WorldObjectShadow : MonoBehaviour
{
    private const string ShadowPivotName = "Shadow_Pivot";
    private const string ShadowVisualName = "Shadow_Visual";

    private SpriteRenderer parentRenderer;
    private SpriteRenderer shadowRenderer;
    private GameObject shadowPivot;
    private GameObject shadowVisual;

    [Header("Foot Position")]
    public Vector2 footOffset = new Vector2(0f, -0.5f);

    [Header("Shadow Settings")]
    public bool isStatic = true;
    public Color shadowColor = new Color(0f, 0f, 0f, 0.4f);
    public Vector2 baseScale = new Vector2(1f, 0.8f);
    [Min(0f)] public float lengthVariation = 0.4f;

    [Header("Sun Direction")]
    [SerializeField] private float morningAngle = 60f;
    [SerializeField] private float eveningAngle = -60f;
    [SerializeField] private int sortingOrderOffset = -1;

    private void Awake()
    {
        parentRenderer = GetComponent<SpriteRenderer>();
        if (parentRenderer == null)
        {
            enabled = false;
            return;
        }

        RebuildShadowObjects();
        SyncShadowRenderer(true);
        UpdateShadowTransform();
    }

    private void LateUpdate()
    {
        if (parentRenderer == null || shadowPivot == null || shadowRenderer == null)
        {
            return;
        }

        bool needsRealign =
            !isStatic ||
            shadowRenderer.sprite != parentRenderer.sprite ||
            shadowRenderer.flipX != parentRenderer.flipX ||
            shadowRenderer.flipY != parentRenderer.flipY;

        SyncShadowRenderer(needsRealign);
        UpdateShadowTransform();
    }

    private void RebuildShadowObjects()
    {
        Transform oldPivot = transform.Find(ShadowPivotName);
        if (oldPivot != null)
        {
            oldPivot.gameObject.SetActive(false);

            if (Application.isPlaying)
            {
                Destroy(oldPivot.gameObject);
            }
            else
            {
                DestroyImmediate(oldPivot.gameObject);
            }
        }

        shadowPivot = new GameObject(ShadowPivotName);
        shadowPivot.transform.SetParent(transform, false);
        shadowPivot.transform.localPosition = footOffset;
        shadowPivot.transform.localRotation = Quaternion.identity;
        shadowPivot.transform.localScale = Vector3.one;

        shadowVisual = new GameObject(ShadowVisualName);
        shadowVisual.transform.SetParent(shadowPivot.transform, false);
        shadowVisual.transform.localRotation = Quaternion.identity;
        shadowVisual.transform.localScale = Vector3.one;

        shadowRenderer = shadowVisual.AddComponent<SpriteRenderer>();
    }

    private void SyncShadowRenderer(bool realignToFoot)
    {
        if (shadowRenderer == null)
        {
            return;
        }

        shadowRenderer.sprite = parentRenderer.sprite;
        shadowRenderer.enabled = parentRenderer.enabled && parentRenderer.sprite != null;
        shadowRenderer.color = shadowColor;
        shadowRenderer.flipX = parentRenderer.flipX;
        shadowRenderer.flipY = parentRenderer.flipY;
        shadowRenderer.drawMode = parentRenderer.drawMode;
        shadowRenderer.size = parentRenderer.size;
        shadowRenderer.sortingLayerID = parentRenderer.sortingLayerID;
        shadowRenderer.sortingOrder = parentRenderer.sortingOrder + sortingOrderOffset;

        if (realignToFoot)
        {
            AlignShadowVisualToFoot();
        }
    }

    private void AlignShadowVisualToFoot()
    {
        if (shadowVisual == null || parentRenderer.sprite == null)
        {
            return;
        }

        Bounds spriteBounds = parentRenderer.sprite.bounds;
        float bottomCenterX = parentRenderer.flipX ? -spriteBounds.center.x : spriteBounds.center.x;
        float bottomY = parentRenderer.flipY ? -spriteBounds.max.y : spriteBounds.min.y;

        shadowVisual.transform.localPosition = new Vector3(-bottomCenterX, -bottomY, 0f);
        shadowVisual.transform.localRotation = Quaternion.identity;
        shadowVisual.transform.localScale = Vector3.one;
    }

    private void UpdateShadowTransform()
    {
        if (shadowPivot == null)
        {
            return;
        }

        float progress = GetDayProgress();
        float angle = Mathf.Lerp(morningAngle, eveningAngle, progress);
        float lengthFactor = Mathf.Abs(progress - 0.5f) * 2f * lengthVariation;

        shadowPivot.transform.localPosition = footOffset;
        shadowPivot.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        shadowPivot.transform.localScale = new Vector3(
            Mathf.Max(0.01f, baseScale.x),
            Mathf.Max(0.01f, baseScale.y + lengthFactor),
            1f
        );
    }

    private float GetDayProgress()
    {
        if (TimeManage.instance != null)
        {
            return Mathf.Repeat(TimeManage.instance.currentTime, 1f);
        }

        if (TimeManagerTick.Instance != null)
        {
            int totalTicks = TimeManagerTick.Instance.dayTicks + TimeManagerTick.Instance.nightTicks;
            if (totalTicks > 0)
            {
                return Mathf.Repeat((float)TimeManagerTick.Instance.currentTick / totalTicks, 1f);
            }
        }

        return 0.5f;
    }
}