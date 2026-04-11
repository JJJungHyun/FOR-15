using UnityEngine;

public class GuideArrow : MonoBehaviour
{
    public Transform player;
    public Transform target; 

    public float radiusX = 1.2f;
    public float radiusY = 2.0f;
    public Vector3 playerOffset = new Vector3(0, 1.0f, 0);
    public float rotationSpeed = 15f;
    public float angleOffset = 0f;
    public bool isItemUsed = true;

    private bool isInsideEscapeZone = false;
    private GameObject visualChild;

    private void Start()
    {
        if (transform.childCount > 0)
            visualChild = transform.GetChild(0).gameObject;
        else
            visualChild = this.gameObject;
    }

    private void Update()
    {
        if (player == null || target == null || visualChild == null) return;

        bool shouldShow = isItemUsed && !isInsideEscapeZone;

        if (visualChild.activeSelf != shouldShow)
            visualChild.SetActive(shouldShow);

        if (shouldShow)
        {
            UpdatePositionAndRotation();
        }
    }

    private void UpdatePositionAndRotation()
    {
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

    public void SetInsideZone(bool inside)
    {
        isInsideEscapeZone = inside;
    }
}