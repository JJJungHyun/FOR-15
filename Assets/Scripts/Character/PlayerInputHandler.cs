using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public static event Action OnInventoryPressed;
    public static event Action OnCraftingPressed;
    public static event Action OnInteractPressed;
    public static event Action<int> OnQuickSlotPressed;
    public static event Action OnAttackPressed;

    private void Update()
    {
        HandleAttack();
        HandleInventory();
        HandleCraftingPanel();
        HandleInteract();
        HandleQuickSlot();
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnAttackPressed?.Invoke();
        }
    }

    private void HandleInventory()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            OnInventoryPressed?.Invoke();
        }
    }

    private void HandleCraftingPanel()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OnCraftingPressed?.Invoke();
        }
    }

    private void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteractPressed?.Invoke();
        }
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
}