using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Ingredient
{
    public Item RequiredItem;
    [Range(1, 99)] public int RequiredAmount;
}

[CreateAssetMenu(fileName = "New Cooking Recipe", menuName = "Recipe/Cooking Recipe")]
public class CookingRecipe : ScriptableObject
{
    public string RecipeName;
    public Item OutputItem;
    [TextArea(2, 5)] public string Description;

    [Header("Ingredients (1~3 types)")]
    public List<Ingredient> Ingredients = new List<Ingredient>();

    public float BaseCookTime = 3f; // 개당 조리 시간(초)
}