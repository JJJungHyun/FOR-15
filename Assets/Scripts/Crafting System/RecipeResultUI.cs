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

        // 1. 결과물 정보 세팅
        if (recipe.Result.Item != null)
        {
            if (resultIconImage != null) resultIconImage.sprite = recipe.Result.Item.Icon;
            if (resultNameText != null) resultNameText.text = recipe.Result.Item.ItemName;
            if (resultDescText != null) resultDescText.text = recipe.Result.Item.GetDescription();
        }

        // 2. 버튼 이벤트 바인딩
        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnCraftButtonClick);
        }

        UpdateRecipeUI();
    }

    // 인벤토리 상태 변화가 있을 때 외부(CraftingPanel)에서 호출해 줄 함수
    // 인벤토리 상태 변화가 있을 때 외부(CraftingPanel)에서 호출해 줄 함수
    public void UpdateRecipeUI()
    {
        // 🛠️ 수정: ingredientParent가 할당되어 있지 않으면 에러를 뿜지 않고 안전하게 리턴하도록 방어 코드 추가
        if (recipe == null || playerInventory == null || ingredientParent == null)
        {
            if (ingredientParent == null)
            {
                Debug.LogWarning($"[RecipeItemUI] '{gameObject.name}' 오브젝트의 ingredientParent가 인스펙터에서 할당되지 않았습니다!");
            }
            return;
        }

        // 1. 재료 슬롯 동적 생성 및 갱신 (1~3개)
        foreach (Transform child in ingredientParent) Destroy(child.gameObject);
        spawnedIngredients.Clear();

        foreach (var material in recipe.Materials)
        {
            if (material.Item == null) continue;
            if (ingredientPrefab == null) continue; // 프리팹 누락 방어

            var ingredientSlot = Instantiate(ingredientPrefab, ingredientParent);
            int currentCount = playerInventory.ItemCount(material.Item.ID);

            ingredientSlot.SetIngredient(material.Item, currentCount, material.Amount);
        }

        // 2. 제작 버튼 활성화 여부 판별
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

            // 제작 후 전체 레시피 목록의 재료 카운트 리프레시 요청
            craftingPanel.RefreshAllRecipesUI();
        }
    }
}