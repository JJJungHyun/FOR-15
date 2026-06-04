using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientItemUI : MonoBehaviour
{
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private TMP_Text ingredientAmountText;

    /// <summary>
    /// 재료 슬롯 UI 초기화 및 인벤토리 수량 검증
    /// </summary>
    /// <returns>요리에 필요한 재료가 충분하면 true, 부족하면 false 반환</returns>
    public bool Init(Item item, int requiredAmountPerOne, int requestCount, ItemContainer inventory)
    {
        if (item == null) return false;

        if (ingredientIcon != null)
            ingredientIcon.sprite = item.Icon;

        // 선택한 요리 수량(requestCount)에 맞춰 필요한 총 재료 수량 계산
        int totalRequired = requiredAmountPerOne * requestCount;

        // 부모인 ItemContainer(Inventory)에 구현된 ItemCount(itemID)를 활용해 현재 가방 보유량 파악
        int currentOwned = (inventory != null) ? inventory.ItemCount(item.ID) : 0;

        bool isEnough = currentOwned >= totalRequired;

        if (ingredientAmountText != null)
        {
            if (isEnough)
            {
                // 재료가 충분할 때: 흰색으로 표현 (예: "3 / 1")
                ingredientAmountText.text = $"{currentOwned} / {totalRequired}";
                ingredientAmountText.color = Color.white;
            }
            else
            {
                // 재료가 부족할 때: 빨간색 서치라이트 피드백
                ingredientAmountText.text = $"{currentOwned} / {totalRequired}";
                ingredientAmountText.color = Color.red;
            }
        }

        return isEnough;
    }
}