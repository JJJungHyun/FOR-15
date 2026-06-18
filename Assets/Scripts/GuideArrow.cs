using UnityEngine;

public class GuideArrow : MonoBehaviour, IActivatableDevice
{
    public string DeviceID => "GuideArrow";

    public Transform player;
    public Transform target;
    [SerializeField] private string targetTag = "Exit";

    public float radiusX = 1.2f;
    public float radiusY = 2.0f;
    public Vector3 playerOffset = new Vector3(0, 1.0f, 0);
    public float rotationSpeed = 15f;
    public float angleOffset = 0f;

    public bool isItemUsed = false;
    private bool isInsideEscapeZone = false;
    private GameObject visualChild;

    private void Awake()
    {
        if (transform.childCount > 0)
            visualChild = transform.GetChild(0).gameObject;
        else
            visualChild = this.gameObject;

        if (visualChild != null) visualChild.SetActive(false);

        if (player == null) player = transform.root;
    }

    private void OnEnable()
    {
        FindTarget();
    }

    private void Update()
    {
        if (!isItemUsed || player == null || target == null || visualChild == null) return;

        bool shouldShow = !isInsideEscapeZone;

        if (visualChild.activeSelf != shouldShow)
            visualChild.SetActive(shouldShow);

        if (shouldShow)
        {
            UpdatePositionAndRotation();
        }
    }

    public void Activate()
    {
        isItemUsed = true;

        if (target == null) FindTarget();

        if (visualChild != null) visualChild.SetActive(true);
        Debug.Log("가이드 화살표 활성화됨");
    }

    public void Deactivate()
    {
        isItemUsed = false;
        if (visualChild != null) visualChild.SetActive(false);
    }

    // 💡 동적 생성을 위해 새로 추가된 메서드: 런타임에 타겟을 직접 주입받음
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log($"[GuideArrow] 새 타겟 주입 완료: {newTarget.name}");
    }

    private void FindTarget()
    {
        if (target != null) return;

        GameObject exitObj = GameObject.FindWithTag(targetTag);
        if (exitObj != null)
        {
            target = exitObj.transform;
            Debug.Log($"[GuideArrow] 타겟 '{exitObj.name}'을 성공적으로 찾았습니다.");
        }
        else
        {
            Debug.LogWarning($"[GuideArrow] '{targetTag}' 태그를 가진 오브젝트를 씬에서 찾을 수 없습니다. (실시간 스폰 아이템일 경우 스포너가 주입해 줄 것입니다)");
        }
    }

    private void UpdatePositionAndRotation()
    {
        // 2D 탑다운/사이드뷰 기준 축 고정 계산
        Vector3 playerCenter = player.position + playerOffset;
        Vector3 dirToTarget = (target.position - playerCenter).normalized;

        dirToTarget.z = 0;

        float posX = dirToTarget.x * radiusX;
        float posY = dirToTarget.y * radiusY;
        transform.position = playerCenter + new Vector3(posX, posY, 0);

        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle + angleOffset);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    public void SetInsideZone(bool inside) => isInsideEscapeZone = inside;
}