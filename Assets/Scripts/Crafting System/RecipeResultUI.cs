using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RecipeItemUI : MonoBehaviour
{
    [Header("Result UI References")]
    [SerializeField] private Image resultIconImage;
    [SerializeField] private TMP_Text resultNameText;
    [SerializeField] private TMP_Text resultDescText;

    [Header("Ingredients UI")]
    [SerializeField] private Transform ingredientParent;
    [SerializeField] private RecipeIngredientUI ingredientPrefab;

    [Header("Craft Button")]
    [SerializeField] private Button craftButton;

    private CraftingRecipe recipe;
    private Inventory playerInventory;
    private CraftingPanel craftingPanel;
    private List<RecipeIngredientUI> spawnedIngredients = new List<RecipeIngredientUI>();

    public void Init(CraftingRecipe newRecipe, Inventory inventory, CraftingPanel panel)
    {
        recipe = newRecipe;
        playerInventory = inventory;
        craftingPanel = panel;

        if (recipe.Result.Item != null)
        {
            if (resultIconImage != null) resultIconImage.sprite = recipe.Result.Item.Icon;
            if (resultNameText != null) resultNameText.text = recipe.Result.Item.ItemName;
            if (resultDescText != null) resultDescText.text = recipe.Result.Item.GetDescription();
        }

        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnCraftButtonClick);
        }

        UpdateRecipeUI();
    }

    public void UpdateRecipeUI()
    {
        if (recipe == null || playerInventory == null || ingredientParent == null)
        {
            if (ingredientParent == null)
            {
                Debug.LogWarning($"[RecipeItemUI] '{gameObject.name}' 오브젝트의 ingredientParent가 인스펙터에서 할당되지 않았습니다!");
            }
            return;
        }

        foreach (Transform child in ingredientParent) Destroy(child.gameObject);
        spawnedIngredients.Clear();

        foreach (var material in recipe.Materials)
        {
            if (material.Item == null) continue;
            if (ingredientPrefab == null) continue; 

            var ingredientSlot = Instantiate(ingredientPrefab, ingredientParent);
            int currentCount = playerInventory.ItemCount(material.Item.ID);

            ingredientSlot.SetIngredient(material.Item, currentCount, material.Amount);
        }

        if (craftButton != null)
        {
            craftButton.interactable = recipe.CanCraft(playerInventory, craftingPanel.CurrentStation);
        }
    }

    private void OnCraftButtonClick()
    {
        if (recipe != null && recipe.CanCraft(playerInventory, craftingPanel.CurrentStation))
        {
            recipe.Craft(playerInventory);

            craftingPanel.RefreshAllRecipesUI();
        }
    }
}