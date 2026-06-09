using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeListItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button clickButton;

    private CookingRecipe targetRecipe;
    private CookingStationUI parentUI;

    public void Init(CookingRecipe recipe, CookingStationUI ui)
    {
        targetRecipe = recipe;
        parentUI = ui;

        if (itemIcon != null) itemIcon.sprite = recipe.OutputItem.Icon;
        if (recipeNameText != null) recipeNameText.text = recipe.RecipeName;
        if (descriptionText != null) descriptionText.text = recipe.Description;

        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(() => parentUI.SelectRecipe(targetRecipe));
    }
}