using UnityEngine;

public class FurnaceInteractable : MonoBehaviour
{
    private bool isPlayerNearby = false;

    private void OnEnable() => PlayerInputHandler.OnInteractPressed += HandleInteract;
    private void OnDisable() => PlayerInputHandler.OnInteractPressed -= HandleInteract;

    private void HandleInteract()
    {
        if (isPlayerNearby && UIManager.Instance != null)
        {
            UIManager.Instance.ToggleFurnaceUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.CloseFurnaceUI();
            }
        }
    }
}