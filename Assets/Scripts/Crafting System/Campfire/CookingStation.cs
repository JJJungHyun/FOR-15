using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    [Header("Station Settings")]
    public List<CookingRecipe> AvailableRecipes;

    // 완성된 결과물이 쌓여 보관되는 독립 슬롯
    public ItemSlot OutputSlot { get; private set; } = new ItemSlot();

    // 현재 조리 및 예약 상태 데이터
    public CookingRecipe CurrentRecipe { get; private set; }
    public int RemainingQueueCount { get; private set; } = 0;
    public float CurrentCookTimer { get; private set; } = 0f;
    public bool IsCooking { get; private set; } = false;

    public event Action OnCookingStateChanged;
    public event Action OnOutputSlotChanged;

    private void Update()
    {
        if (!IsCooking || CurrentRecipe == null) return;

        CurrentCookTimer += Time.deltaTime;
        OnCookingStateChanged?.Invoke();

        if (CurrentCookTimer >= CurrentRecipe.BaseCookTime)
        {
            CompleteCurrentItem();
        }
    }

    public void StartCooking(CookingRecipe recipe, int requestCount, Inventory playerInventory)
    {
        if (recipe == null || requestCount <= 0) return;

        // 실제 플레이어 가방에서 예약 개수만큼의 총 재료를 뺍니다.
        if (!ConsumeIngredients(recipe, requestCount, playerInventory)) return;

        CurrentRecipe = recipe;
        RemainingQueueCount = requestCount;
        CurrentCookTimer = 0f;
        IsCooking = true;

        OnCookingStateChanged?.Invoke();
    }

    private bool ConsumeIngredients(CookingRecipe recipe, int count, Inventory inventory)
    {
        // 1차 검증: 모든 재료가 충분히 있는지 확인
        foreach (var ing in recipe.Ingredients)
        {
            int totalNeeded = ing.RequiredAmount * count;
            int foundAmount = 0;

            // 인벤토리 전체 뒤져서 개수 파악
            foreach (var slot in inventory.itemSlots)
            {
                if (slot.Item != null && slot.Item.ID == ing.RequiredItem.ID)
                    foundAmount += slot.Amount;
            }
            if (foundAmount < totalNeeded) return false;
        }

        // 2차 실행: 실제 재료 차감
        foreach (var ing in recipe.Ingredients)
        {
            int amountToRemove = ing.RequiredAmount * count;
            foreach (var slot in inventory.itemSlots)
            {
                if (slot.Item != null && slot.Item.ID == ing.RequiredItem.ID)
                {
                    if (slot.Amount >= amountToRemove)
                    {
                        slot.Amount -= amountToRemove;
                        amountToRemove = 0;
                    }
                    else
                    {
                        amountToRemove -= slot.Amount;
                        slot.Amount = 0;
                    }

                    if (slot.Amount <= 0) slot.Item = null;
                    slot.UpdateSlot();

                    if (amountToRemove <= 0) break;
                }
            }
        }
        return true;
    }

    private void CompleteCurrentItem()
    {
        if (CurrentRecipe == null) return;

        // 결과물 슬롯에 아이템 누적 가산
        if (OutputSlot.Item == null)
        {
            OutputSlot.Item = CurrentRecipe.OutputItem.GetCopy();
            OutputSlot.Amount = 1;
        }
        else if (OutputSlot.Item.ID == CurrentRecipe.OutputItem.ID)
        {
            OutputSlot.Amount = Mathf.Min(OutputSlot.Item.MaximumStacks, OutputSlot.Amount + 1);
        }

        OutputSlot.UpdateSlot();
        OnOutputSlotChanged?.Invoke();

        // 대기열 차감
        RemainingQueueCount--;
        CurrentCookTimer = 0f;

        if (RemainingQueueCount <= 0)
        {
            StopCooking(false);
        }
        else
        {
            OnCookingStateChanged?.Invoke();
        }
    }

    public void CancelCooking(Inventory playerInventory)
    {
        if (!IsCooking || CurrentRecipe == null) return;

        // 아직 만들어지지 않고 '대기 대기중인 예약 재료'들은 전부 플레이어 가방으로 돌려줍니다.
        if (RemainingQueueCount > 0)
        {
            foreach (var ing in CurrentRecipe.Ingredients)
            {
                int refundAmount = ing.RequiredAmount * RemainingQueueCount;
                playerInventory.AddItemCustomPriority(ing.RequiredItem, refundAmount);
            }
        }

        StopCooking(true);
    }

    private void StopCooking(bool isCanceled)
    {
        IsCooking = false;
        CurrentRecipe = null;
        RemainingQueueCount = 0;
        CurrentCookTimer = 0f;
        OnCookingStateChanged?.Invoke();
    }

    public void ClearOutputSlot()
    {
        OutputSlot.Item = null;
        OutputSlot.Amount = 0;
        OutputSlot.UpdateSlot();
        OnOutputSlotChanged?.Invoke();
    }
}