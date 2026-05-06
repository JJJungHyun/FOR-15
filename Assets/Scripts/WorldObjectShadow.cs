using UnityEngine;

public class WorldObjectShadow : MonoBehaviour
{
    private SpriteRenderer parentRenderer;
    private SpriteRenderer shadowRenderer;
    private GameObject shadowPivot;
    private GameObject shadowVisual;

    [Header("발바닥 정중앙 위치 (Offset)")]
    public Vector2 footOffset = new Vector2(0f, -0.5f);

    [Header("그림자 설정")]
    public bool isStatic = true;
    public Color shadowColor = new Color(0, 0, 0, 0.4f); // 뒤쪽이라 조금 더 연하게 설정 권장
    public Vector2 baseScale = new Vector2(1f, 0.8f);
    public float lengthVariation = 0.4f;

    void Start()
    {
        parentRenderer = GetComponent<SpriteRenderer>();
        if (parentRenderer == null) return;

        Transform oldPivot = transform.Find("Shadow_Pivot");
        if (oldPivot != null) Destroy(oldPivot.gameObject);

        shadowPivot = new GameObject("Shadow_Pivot");
        shadowPivot.transform.SetParent(transform);
        shadowPivot.transform.localPosition = footOffset;

        shadowVisual = new GameObject("Shadow_Visual");
        shadowVisual.transform.SetParent(shadowPivot.transform);

        shadowRenderer = shadowVisual.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = parentRenderer.sprite;
        shadowRenderer.color = shadowColor;
        // 캐릭터 본체보다 한 단계 뒤에 렌더링
        shadowRenderer.sortingOrder = parentRenderer.sortingOrder - 1;

        // [핵심] 그림자가 위(뒤)쪽으로 뻗도록 설정
        SetupShadowVisual();

        if (isStatic) SyncSprite();
    }

    void LateUpdate()
    {
        if (shadowPivot == null) return;
        shadowPivot.transform.localPosition = footOffset;

        if (!isStatic) SyncSprite();
        UpdateShadowTransform();
    }

    void SetupShadowVisual()
    {
        if (parentRenderer == null || shadowVisual == null) return;

        // 스프라이트 높이의 절반
        float halfHeight = parentRenderer.sprite.bounds.size.y / 2f;

        // [변경점] 이미지를 위쪽(+)으로 밀어올려서 '발' 부분이 피벗에 오게 함
        shadowVisual.transform.localPosition = new Vector3(0, halfHeight, 0);

        // [변경점] 스케일을 양수(1)로 두어 원래 모습 그대로 위로 뻗게 함
        shadowVisual.transform.localScale = new Vector3(1, 1, 1);
    }

    void SyncSprite()
    {
        if (parentRenderer == null || shadowRenderer == null) return;
        shadowRenderer.sprite = parentRenderer.sprite;
        shadowRenderer.flipX = parentRenderer.flipX;
    }

    void UpdateShadowTransform()
    {
        if (TimeManage.instance == null || shadowPivot == null) return;

        float progress = TimeManage.instance.currentTime;

        // 1. 회전: 발바닥을 축으로 등 뒤에서 좌우로 기울어짐
        // 해의 이동 방향에 맞춰 각도를 반대로(60 -> -60) 조절하면 더 자연스럽습니다.
        float angle = Mathf.Lerp(60f, -60f, progress);
        shadowPivot.transform.localRotation = Quaternion.Euler(0, 0, angle);

        // 2. 크기 조절: 발바닥에서 위쪽 방향으로 길어짐
        float lengthFactor = (Mathf.Abs(progress - 0.5f) * 2f) * lengthVariation;
        shadowPivot.transform.localScale = new Vector3(baseScale.x, baseScale.y + lengthFactor, 1f);
    }
}