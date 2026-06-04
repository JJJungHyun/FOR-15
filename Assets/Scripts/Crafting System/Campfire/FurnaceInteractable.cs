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
        // 플레이어가 범위 내에 있고, 연동할 화로 데이터가 존재할 때만 실행
        if (isPlayerNearby && cookingStation != null)
        {
            // 씬에 존재하는 CookingStationUI를 찾아서 오픈 요청과 함께 데이터를 주입합니다.
            CookingStationUI cookingUI = FindAnyObjectByType<CookingStationUI>(FindObjectsInactive.Include);

            if (cookingUI != null)
            {
                // UI가 이미 켜져 있다면 닫고, 꺼져 있다면 해당 화로 데이터로 새로고침하며 엽니다.
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

            // 플레이어가 화로에서 멀어지면 열려있던 화로 UI창을 안전하게 강제 종료시킵니다.
            CookingStationUI cookingUI = FindAnyObjectByType<CookingStationUI>(FindObjectsInactive.Include);
            if (cookingUI != null && cookingUI.gameObject.activeSelf)
            {
                cookingUI.CloseStationUI();
            }
        }
    }
}