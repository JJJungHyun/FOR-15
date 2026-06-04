using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPanel : MonoBehaviour
{
    [Header("Data References")]
    [SerializeField] private List<CraftingRecipe> allRecipes;
    [SerializeField] private Inventory playerInventory;

    [Header("Scroll Area Links")]
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private RecipeItemUI recipeItemPrefab; 

    [Header("Category Navigation")]
    [SerializeField] private List<CategoryTabBtn> categoryButtons;

    private CraftingStation currentStation = CraftingStation.None;
    private CraftingCategory currentCategory = CraftingCategory.All;
    private List<RecipeItemUI> spawnedRecipeUIs = new List<RecipeItemUI>();

    public CraftingStation CurrentStation => currentStation;

    private void Start()
    {
        if (playerInventory != null)
        {
            playerInventory.OnItemsChanged += RefreshAllRecipesUI;
        }

        InitCategoryTabs();
        RefreshRecipeList();
    }

    private void OnEnable()
    {
        RefreshRecipeList();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnItemsChanged -= RefreshAllRecipesUI;
        }
    }

    private void InitCategoryTabs()
    {
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            if (categoryButtons[i] == null) continue;
            categoryButtons[i].Init(this, i);
        }
        UpdateCategoryTabHighlights();
    }

    public void SetCategory(int categoryIndex)
    {
        currentCategory = (CraftingCategory)categoryIndex;
        UpdateCategoryTabHighlights();
        RefreshRecipeList();
    }

    private void UpdateCategoryTabHighlights()
    {
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            if (categoryButtons[i] != null)
            {
                categoryButtons[i].SetSelectState((int)currentCategory == i);
            }
        }
    }

    public void RefreshRecipeList()
    {
        foreach (var ui in spawnedRecipeUIs) Destroy(ui.gameObject);
        spawnedRecipeUIs.Clear();

        if (playerInventory == null || recipeItemPrefab == null) return;

        foreach (var recipe in allRecipes)
        {
            if (recipe == null) continue;

            bool isCorrectCategory = (currentCategory == CraftingCategory.All) || (recipe.Category == currentCategory);
            bool isCorrectStation = (recipe.RequiredStation == CraftingStation.None) || (recipe.RequiredStation == currentStation);

            if (isCorrectCategory && isCorrectStation)
            {
                var recipeUI = Instantiate(recipeItemPrefab, recipeListParent);
                recipeUI.Init(recipe, playerInventory, this);
                spawnedRecipeUIs.Add(recipeUI);
            }
        }
    }

    public void RefreshAllRecipesUI()
    {
        foreach (var ui in spawnedRecipeUIs)
        {
            if (ui != null) ui.UpdateRecipeUI();
        }
    }
}