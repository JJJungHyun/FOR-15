using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CraftingPanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<CraftingRecipe> allRecipes;
    [SerializeField] private Inventory playerInventory;

    [Header("List UI")]
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private RecipeResultUI recipeButtonPrefab;

    [Header("Detail Window")]
    [SerializeField] private GameObject detailWindow;
    [SerializeField] private TMP_Text resultNameText;
    [SerializeField] private TMP_Text resultDescText;
    [SerializeField] private Transform ingredientParent;
    [SerializeField] private RecipeIngredientUI ingredientPrefab;
    [SerializeField] private Button craftButton;

    private CraftingRecipe selectedRecipe;
    private CraftingStation currentStation = CraftingStation.None;
    private CraftingCategory currentCategory = CraftingCategory.All;
    private List<RecipeResultUI> spawnedButtons = new List<RecipeResultUI>();

    private void Start()
    {
        RefreshRecipeList();
    }

    private void OnEnable()
    {
        RefreshRecipeList();
    }

    public void SetCategory(int categoryIndex)
    {
        currentCategory = (CraftingCategory)categoryIndex;
        RefreshRecipeList();
    }

    public void RefreshRecipeList()
    {
        foreach (var btn in spawnedButtons) Destroy(btn.gameObject);
        spawnedButtons.Clear();

        foreach (var recipe in allRecipes)
        {
            // 카테고리 필터링 
            bool isCorrectCategory = (currentCategory == CraftingCategory.All) || (recipe.Category == currentCategory);

            if (isCorrectCategory)
            {
                if (recipe.RequiredStation != CraftingStation.None && recipe.RequiredStation != currentStation)
                    continue;

                var btn = Instantiate(recipeButtonPrefab, recipeListParent);
                btn.Init(recipe, this);
                btn.UpdateAvailability(playerInventory, currentStation);
                spawnedButtons.Add(btn);
            }
        }

        if (selectedRecipe != null) SelectRecipe(selectedRecipe);
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        detailWindow.SetActive(true);

        resultNameText.text = recipe.Result.Item.ItemName;
        resultDescText.text = recipe.Result.Item.GetDescription();

        foreach (Transform child in ingredientParent) Destroy(child.gameObject);

        foreach (var material in recipe.Materials)
        {
            var slot = Instantiate(ingredientPrefab, ingredientParent);
            int currentCount = playerInventory.ItemCount(material.Item.ID);
            slot.SetIngredient(material.Item, currentCount, material.Amount);
        }

        craftButton.interactable = recipe.CanCraft(playerInventory, currentStation);
    }

    public void OnCraftClick()
    {
        if (selectedRecipe != null && selectedRecipe.CanCraft(playerInventory, currentStation))
        {
            selectedRecipe.Craft(playerInventory);
            RefreshRecipeList();
        }
    }
}