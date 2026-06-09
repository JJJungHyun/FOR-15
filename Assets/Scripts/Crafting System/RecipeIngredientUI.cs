using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class RecipeIngredientUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color insufficientColor = Color.red;

    public void SetIngredient(Item item, int current, int required)
    {
        iconImage.sprite = item.Icon;
        countText.text = $"{current}/{required}";

        countText.color = (current >= required) ? normalColor : insufficientColor;
        iconImage.color = (current >= required) ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
    }
}