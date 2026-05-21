using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : ItemContainer
{
    [Header("Furnace Settings")]
    public List<SmeltingRecipe> Recipes;
    
    private const int INPUT_SLOT = 0;
    private const int OUTPUT_SLOT = 1; 

    private float _cookingTimer = 0f;
    private bool _isCooking = false;

    private void Awake()
    {
        if (itemSlots.Count == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                itemSlots.Add(new ItemSlot());
            }
        }
    }

    private void Update()
    {
        if (CanSmelt())
        {
            StartCooking();
        }
        else
        {
            StopCooking();
        }
    }

    public bool IsCooking => _isCooking;

    private bool CanSmelt()
    {
        ItemSlot input = itemSlots[INPUT_SLOT];
        ItemSlot output = itemSlots[OUTPUT_SLOT];

        if (input.Item == null) return false;

        SmeltingRecipe recipe = GetRecipeFor(input.Item);
        if (recipe == null) return false;

        if (output.Item != null)
        {
            if (output.Item.ID != recipe.OutputItem.ID) return false;
            if (output.Amount >= output.Item.MaximumStacks) return false;
        }

        return true;
    }

    private void StartCooking()
    {
        _isCooking = true;
        _cookingTimer += Time.deltaTime;

        SmeltingRecipe recipe = GetRecipeFor(itemSlots[INPUT_SLOT].Item);
        
        if (_cookingTimer >= recipe.CookTime)
        {
            CompleteSmelting(recipe);
            _cookingTimer = 0f;
        }
    }

    private void StopCooking()
    {
        _isCooking = false;
        _cookingTimer = 0f;
    }

    private void CompleteSmelting(SmeltingRecipe recipe)
    {
        itemSlots[INPUT_SLOT].Amount--;
        if (itemSlots[INPUT_SLOT].Amount <= 0) itemSlots[INPUT_SLOT].Item = null;
        itemSlots[INPUT_SLOT].UpdateSlot();

        if (itemSlots[OUTPUT_SLOT].Item == null)
        {
            itemSlots[OUTPUT_SLOT].Item = recipe.OutputItem.GetCopy();
            itemSlots[OUTPUT_SLOT].Amount = 1;
        }
        else
        {
            itemSlots[OUTPUT_SLOT].Amount++;
        }
        itemSlots[OUTPUT_SLOT].UpdateSlot();
    }

    private SmeltingRecipe GetRecipeFor(Item item)
    {
        if (item == null || Recipes == null) return null;

        return Recipes.Find(r =>
            r != null &&               
            r.InputItem != null && 
            r.InputItem.ID == item.ID 
        );
    }

    public SmeltingRecipe GetCurrentRecipe()
    {
        if (itemSlots[INPUT_SLOT].Item == null) return null;
        return GetRecipeFor(itemSlots[INPUT_SLOT].Item);
    }


    // 현재 진행률
    public float GetProgress()
    {
        if (!_isCooking) return 0;

        Item inputItem = itemSlots[INPUT_SLOT].Item;
        if (inputItem == null) return 0;

        SmeltingRecipe recipe = GetRecipeFor(inputItem);
        return (recipe != null) ? _cookingTimer / recipe.CookTime : 0;
    }
}