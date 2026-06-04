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
    [SerializeField] private RecipeItemUI recipeItemPrefab; // 새로 바뀐 프리팹

    [Header("Category Navigation")]
    [SerializeField] private List<CategoryTabBtn> categoryButtons;

    private CraftingStation currentStation = CraftingStation.None;
    private CraftingCategory currentCategory = CraftingCategory.All;
    private List<RecipeItemUI> spawnedRecipeUIs = new List<RecipeItemUI>();

    public CraftingStation CurrentStation => currentStation;

    private void Start()
    {
        // 인벤토리 변경 이벤트 구독 (실시간 재료 개수 리프레시용)
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
        // 기존 리스트 오브젝트 풀링 없이 전부 파괴 후 재생성
        foreach (var ui in spawnedRecipeUIs) Destroy(ui.gameObject);
        spawnedRecipeUIs.Clear();

        if (playerInventory == null || recipeItemPrefab == null) return;

        foreach (var recipe in allRecipes)
        {
            if (recipe == null) continue;

            // 카테고리 필터링 
            bool isCorrectCategory = (currentCategory == CraftingCategory.All) || (recipe.Category == currentCategory);
            // 작업대 필터링
            bool isCorrectStation = (recipe.RequiredStation == CraftingStation.None) || (recipe.RequiredStation == currentStation);

            if (isCorrectCategory && isCorrectStation)
            {
                var recipeUI = Instantiate(recipeItemPrefab, recipeListParent);
                recipeUI.Init(recipe, playerInventory, this);
                spawnedRecipeUIs.Add(recipeUI);
            }
        }
    }

    // 아이템 제작이나 인벤토리 변동 시 호출되어 화면에 뜬 모든 레시피 재료 상황을 동기화
    public void RefreshAllRecipesUI()
    {
        foreach (var ui in spawnedRecipeUIs)
        {
            if (ui != null) ui.UpdateRecipeUI();
        }
    }
}