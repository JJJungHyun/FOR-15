using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    public Item Item;
    [Range(1, 999)]
    public int Amount;
}

public enum CraftingCategory { All, Weapon, Armor, Food, Misc }
public enum CraftingStation { None, Workbench, Forge }

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public CraftingCategory Category;
    public CraftingStation RequiredStation;

    public List<ItemAmount> Materials;
    public ItemAmount Result;

    public bool CanCraft(IItemContainer container, CraftingStation currentStation)
    {
        if (RequiredStation != currentStation && RequiredStation != CraftingStation.None) return false;
        if (container.IsFull()) return false;

        foreach (var material in Materials)
        {
            if (container.ItemCount(material.Item.ID) < material.Amount) return false;
        }
        return true;
    }

    public void Craft(IItemContainer container)
    {
        // 재료 제거
        foreach (var material in Materials)
        {
            for (int i = 0; i < material.Amount; i++)
            {
                container.RemoveItemByID(material.Item.ID);
            }
        }

        // 결과물 추가
        for (int i = 0; i < Result.Amount; i++)
        {
            container.AddItem(Result.Item.GetCopy());
        }
    }
}