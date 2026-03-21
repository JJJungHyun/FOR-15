using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public static event Action OnInventoryPressed;
    public static event Action OnInteractPressed;
    public static event Action<int> OnQuickSlotPressed;

    private void Update()
    {
        HandleInventory();
        HandleInteract();
        HandleQuickSlot();
    }

    private void HandleInventory()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
        {
            OnInventoryPressed?.Invoke();
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
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                OnQuickSlotPressed?.Invoke(i);
            }
        }
    }
}