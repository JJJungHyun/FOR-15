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

[CreateAssetMenu(menuName = "Recipe/Crafting Recipe")]

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

        if (Result.Item != null)
        {
            // 1. 제작 신호 로그
            Debug.Log($"[제작 완료] 아이템: {Result.Item.name}");

            // 2. 맵에 있는 모든 Spawner에게 제작 소식을 알립니다.
            // 각 Spawner는 자기가 기다리던 아이템인지 스스로 판단하게 됩니다.
            EscapeRouteSpawner[] spawners = FindObjectsOfType<EscapeRouteSpawner>();
            foreach (var spawner in spawners)
            {
                spawner.CheckAndActivate(Result.Item);
            }

            if (CutSceneManager.Instance != null)
            {
                if (Result.Item.name == "EscapeCore")
                {
                    CutSceneManager.Instance.StartCutscene("GuideArrowCrafted", "Escape");
                }
            }
        }
    }
}