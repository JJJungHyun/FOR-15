using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
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