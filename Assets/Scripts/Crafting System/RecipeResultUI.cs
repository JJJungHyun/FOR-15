using UnityEngine;
using UnityEngine.UI;

public class RecipeResultUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private CraftingRecipe recipe;
    private CraftingPanel panel;

    public void Init(CraftingRecipe _recipe, CraftingPanel _panel)
    {
        recipe = _recipe;
        panel = _panel;
        iconImage.sprite = recipe.Result.Item.Icon;
    }

    // 제작 가능 여부에 따라 밝기 조절
    public void UpdateAvailability(IItemContainer container, CraftingStation currentStation)
    {
        bool canCraft = recipe.CanCraft(container, currentStation);
        canvasGroup.alpha = canCraft ? 1.0f : 0.4f; 
    }

    public void OnClick()
    {
        panel.SelectRecipe(recipe); 
    }
}
