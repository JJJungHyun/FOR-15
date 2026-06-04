using UnityEngine;

[RequireComponent(typeof(CookingStation))]
public class FurnaceInteractable : MonoBehaviour
{
    private CookingStation cookingStation;
    private bool isPlayerNearby = false;

    private void Awake()
    {
        cookingStation = GetComponent<CookingStation>();
    }

    private void OnEnable() => PlayerInputHandler.OnInteractPressed += HandleInteract;
    private void OnDisable() => PlayerInputHandler.OnInteractPressed -= HandleInteract;

    private void HandleInteract()
    {
        if (isPlayerNearby && cookingStation != null)
        {
            CookingStationUI cookingUI = FindAnyObjectByType<CookingStationUI>(FindObjectsInactive.Include);

            if (cookingUI != null)
            {
                if (cookingUI.gameObject.activeSelf)
                {
                    cookingUI.CloseStationUI();
                }
                else
                {
                    cookingUI.OpenStationUI(cookingStation);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            CookingStationUI cookingUI = FindAnyObjectByType<CookingStationUI>(FindObjectsInactive.Include);
            if (cookingUI != null && cookingUI.gameObject.activeSelf)
            {
                cookingUI.CloseStationUI();
            }
        }
    }
}