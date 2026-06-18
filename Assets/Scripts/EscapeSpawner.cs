using UnityEngine;

public class EscapeRouteSpawner : MonoBehaviour
{
    [Header("Item To Wait For")]
    [Tooltip("이 스포너를 활성화할 아이템 이름 (예: EscapeCore 또는 escape_C)")]
    [SerializeField] private string targetItemName;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject escapePortalPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Arrow Indicator")]
    [SerializeField] private GameObject directionArrow;

    private GameObject spawnedPortal;
    private bool isActivated = false;

    private void Start()
    {
        if (directionArrow != null) directionArrow.SetActive(false);
    }

    // ★ Recipe에서 호출되는 함수
    public void CheckAndActivate(Item craftedItem)
    {
        if (isActivated) return;

        // 제작된 아이템 이름이 내가 기다리던 이름과 일치하는지 확인
        if (craftedItem.name == targetItemName || craftedItem.ID == targetItemName)
        {
            ActivateSpawner();
        }
    }

    private void ActivateSpawner()
    {
        isActivated = true;
        Debug.Log($"[Spawner] {targetItemName} 일치 확인! 탈출구 가동.");

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
            if (arrowScript != null && spawnedPortal != null)
            {
                arrowScript.SetTarget(spawnedPortal.transform);
            }
        }
    }
}