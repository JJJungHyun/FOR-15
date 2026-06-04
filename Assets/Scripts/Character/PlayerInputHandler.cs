using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    // 인벤토리와 제작창 통합 토글 이벤트 하나로 통일
    public static event Action OnToggleCombinedUI;
    public static event Action OnInteractPressed;
    public static event Action<int> OnQuickSlotPressed;

    public static event Action OnAttackPressed;
    public static event Action OnAttackHeld;
    public static event Action OnAttackReleased;
    public static event Action OnFarmingPressed;

    private void Update()
    {
        HandleAttack();
        HandleCombinedUI();
        HandleInteract();
        HandleQuickSlot();
        HandleFarming();
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) OnAttackPressed?.Invoke();
        if (Input.GetMouseButton(0)) OnAttackHeld?.Invoke();
        if (Input.GetMouseButtonUp(0)) OnAttackReleased?.Invoke();
    }

    private void HandleCombinedUI()
    {
        // I, Tab, B 중 무엇을 누르든 한 번에 켜고 꺼지도록 동일한 이벤트를 날립니다.
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.B))
        {
            OnToggleCombinedUI?.Invoke();
        }
    }

    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E)) OnInteractPressed?.Invoke();
    }

    private void HandleQuickSlot()
    {
        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                OnQuickSlotPressed?.Invoke(i);
            }
        }
    }

    private void HandleFarming()
    {
        if (Input.GetKeyDown(KeyCode.F)) OnFarmingPressed?.Invoke();
    }
}