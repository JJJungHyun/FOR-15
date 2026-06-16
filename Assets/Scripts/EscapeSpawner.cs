using UnityEngine;

public class EscapeRouteSpawner : MonoBehaviour
{
    [Header("Escape Phase Setting")]
    [SerializeField] private EscapePhase targetPhase;

    [Tooltip("탈출을 활성화할 아이템의 ID 또는 ItemName")]
    [SerializeField] private string targetItemName;

    [Header("Spawn Settings")]
    [Tooltip("탈출구 프리팹")]
    [SerializeField] private GameObject escapePortalPrefab;
    [Tooltip("탈출구 생성 위치")]
    [SerializeField] private Transform spawnPoint;

    [Header("Arrow Indicator")]
    [Tooltip("화살표 오브젝트")]
    [SerializeField] private GameObject directionArrow;

    private GameObject spawnedPortal;
    private bool isActivated = false;

    private void OnEnable()
    {
        ItemObject.OnAnyItemPickedUp += HandleItemPickedUp;
    }

    private void OnDisable()
    {
        ItemObject.OnAnyItemPickedUp -= HandleItemPickedUp;
    }

    private void Start()
    {
        if (directionArrow != null) directionArrow.SetActive(false);
    }

    private void HandleItemPickedUp(Item pickedItem)
    {
        if (pickedItem == null || targetPhase == EscapePhase.None || string.IsNullOrEmpty(targetItemName)) return;

        bool isTargetItem = (pickedItem.ID == targetItemName || pickedItem.ItemName == targetItemName || pickedItem.name == targetItemName);

        if (isTargetItem)
        {
            ActivateSpawner();
        }
    }

    private void ActivateSpawner()
    {
        if (isActivated) return;
        isActivated = true;

        if (escapePortalPrefab != null)
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            spawnedPortal = Instantiate(escapePortalPrefab, pos, rot);
        }

        if (directionArrow != null)
        {
            directionArrow.SetActive(true);

            GuideArrow arrowScript = directionArrow.GetComponent<GuideArrow>();
            IActivatableDevice arrowDevice = directionArrow.GetComponent<IActivatableDevice>();

            if (arrowScript != null && spawnedPortal != null)
            {
                arrowScript.SetTarget(spawnedPortal.transform);
            }

            if (arrowDevice != null) arrowDevice.Activate();
        }
    }
}