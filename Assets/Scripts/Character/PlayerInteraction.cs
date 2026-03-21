using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private LayerMask itemLayer;

    private void OnEnable() => PlayerInputHandler.OnInteractPressed += TryPickupItem;
    private void OnDisable() => PlayerInputHandler.OnInteractPressed -= TryPickupItem;

    private void TryPickupItem()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, itemLayer);

        float closestDistance = Mathf.Infinity;
        ItemObject closestItem = null;

        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent(out ItemObject itemObj))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = itemObj;
                }
            }
        }

        if (closestItem != null)
        {
            if (inventory.AddItemCustomPriority(closestItem.GetItem(), closestItem.GetAmount()))
            {
                closestItem.OnPickedUp();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}