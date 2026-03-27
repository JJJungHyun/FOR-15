using UnityEngine;
using UnityEngine.UI;

public class RecipeResultUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image slotBackgroundImage; 
    [SerializeField] private Image iconImage; 
    [SerializeField] private CanvasGroup canvasGroup;

    private CraftingRecipe recipe;
    private CraftingPanel panel;

    public void Init(CraftingRecipe _recipe, CraftingPanel _panel)
    {
        recipe = _recipe;
        panel = _panel;

        if (iconImage != null && recipe.Result.Item != null)
        {
            iconImage.sprite = recipe.Result.Item.Icon;
            iconImage.gameObject.SetActive(true); 
        }
    }

    public void UpdateAvailability(IItemContainer container, CraftingStation currentStation)
    {
        if (recipe == null) return;

        bool canCraft = recipe.CanCraft(container, currentStation);
        canvasGroup.alpha = canCraft ? 1.0f : 0.4f;
    }

    public void OnClick()
    {
        panel.SelectRecipe(recipe);
    }
}