using UnityEngine;

[CreateAssetMenu(menuName = "Items/Deployable Item")]
public class DeployableItem : Item
{
    [Header("Deployment Settings")]
    [Tooltip("필드에 실제 설치될 오브젝트의 프리팹")]
    public GameObject DeployablePrefab;

    [Tooltip("설치 가능한 최대 거리 (플레이어 기준)")]
    public float MaxPlacementDistance = 5f;

    public override void Use(Character c)
    {
        // 플레이어에게 부착된 배치를 담당하는 컴포넌트나 글로벌 매니저를 호출합니다.
        PlacementManager placementManager = FindFirstObjectByType<PlacementManager>();
        if (placementManager != null)
        {
            placementManager.StartPlacementMode(this, c.transform);
        }
    }

    public override string GetItemType() => "설치물";
    public override string GetDescription() => $"{ItemName}\n지정된 위치에 설치할 수 있습니다.";
}