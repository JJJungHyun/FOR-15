using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientItemUI : MonoBehaviour
{
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private TMP_Text ingredientAmountText;


    public bool Init(Item item, int requiredAmountPerOne, int requestCount, ItemContainer inventory)
    {
        if (item == null) return false;

        if (ingredientIcon != null)
            ingredientIcon.sprite = item.Icon;

        int totalRequired = requiredAmountPerOne * requestCount;

        int currentOwned = (inventory != null) ? inventory.ItemCount(item.ID) : 0;

        bool isEnough = currentOwned >= totalRequired;

        if (ingredientAmountText != null)
        {
            if (isEnough)
            {
                ingredientAmountText.text = $"{currentOwned} / {totalRequired}";
                ingredientAmountText.color = Color.white;
            }
            else
            {
                ingredientAmountText.text = $"{currentOwned} / {totalRequired}";
                ingredientAmountText.color = Color.red;
            }
        }

        return isEnough;
    }
}