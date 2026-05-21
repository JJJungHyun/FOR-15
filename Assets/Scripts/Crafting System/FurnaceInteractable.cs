using UnityEngine;

public class FurnaceInteractable : MonoBehaviour
{
    [SerializeField] private GameObject furnaceCanvas; // 화로 UI 패널
    private bool isPlayerNearby = false;

    private void OnEnable() => PlayerInputHandler.OnInteractPressed += HandleInteract;
    private void OnDisable() => PlayerInputHandler.OnInteractPressed -= HandleInteract;

    private void HandleInteract()
    {
        if (isPlayerNearby)
        {
            // UI 토글
            bool isActive = furnaceCanvas.activeSelf;
            furnaceCanvas.SetActive(!isActive);

            // UI가 켜질 때 인벤토리도 같이 켜주면 편함 (기존 인벤토리 이벤트 호출 가능)
        }
    }

    // 플레이어가 근처에 있는지 체크 (화로에 Trigger 콜라이더 필요)
    private void OnTriggerEnter2D(Collider2D other) // 3D라면 Collider
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            furnaceCanvas.SetActive(false); // 멀어지면 닫기
        }
    }
}