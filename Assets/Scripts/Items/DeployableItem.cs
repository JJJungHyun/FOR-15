using UnityEngine;

[CreateAssetMenu(menuName = "Items/Deployable Item")]
public class DeployableItem : Item
{
    [Header("Deployment Settings")]
    public GameObject DeployablePrefab;

    [Tooltip("설치 가능한 최대 거리")]
    public float MaxPlacementDistance = 5f;

    public override void Use(Character c)
    {
        PlacementManager placementManager = FindFirstObjectByType<PlacementManager>();
        if (placementManager != null)
        {
            placementManager.StartPlacementMode(this, c.transform);
        }
    }

    public override string GetItemType() => "설치물";
    public override string GetDescription() => $"{ItemName}\n지정된 위치에 설치할 수 있습니다.";
}