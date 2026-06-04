using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingStationUI : MonoBehaviour
{
    [Header("Connected Core Logic")]
    [SerializeField] private Inventory playerInventory;
    private CookingStation currentStation;

    [Header("Left Pane - Recipe List Area")]
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private RecipeListItemUI recipeItemPrefab;

    [Header("Right Pane - Selected Recipe Panel")]
    [SerializeField] private GameObject detailAreaRoot;
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private TMP_Text selectedItemNameText;

    [Header("Ingredient UI Area (Prefab Dynamic Spawn)")]
    [SerializeField] private Transform ingredientListParent;
    [SerializeField] private IngredientItemUI ingredientPrefab;

    [Header("Quantity Selector Control")]
    [SerializeField] private TMP_Text requestCountText;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button cookStartButton;

    [Header("Cooking Status HUD Zone (Image Fill Amount)")]
    [SerializeField] private GameObject cookingProgressRoot;
    // 🛠️ Slider에서 Image 타입으로 변경 (인스펙터에서 Fill Method가 세팅된 Image를 연결하세요)
    [SerializeField] private Image timerFillImage;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private Button cancelButton;

    [Header("Result Slot Area")]
    [SerializeField] private BaseItemSlotUI resultSlotUI;

    private CookingRecipe selectedRecipe;
    private int currentRequestCount = 1;
    private List<IngredientItemUI> spawnedIngredientUIs = new List<IngredientItemUI>();

    private void Awake()
    {
        if (plusButton != null) plusButton.onClick.AddListener(() => ChangeQuantity(1));
        if (minusButton != null) minusButton.onClick.AddListener(() => ChangeQuantity(-1));
        if (cookStartButton != null) cookStartButton.onClick.AddListener(OnClickStartCook);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnClickCancelCook);
    }

    private void OnEnable()
    {
        if (playerInventory == null)
        {
            playerInventory = FindAnyObjectByType<Inventory>();
        }
    }

    private void Update()
    {
        UpdateCookingProgressUI();
    }

    public void OpenStationUI(CookingStation station)
    {
        currentStation = station;
        gameObject.SetActive(true);

        currentStation.OnCookingStateChanged += UpdateCookingProgressUI;
        currentStation.OnOutputSlotChanged += UpdateResultSlotUI;

        GenerateRecipeList();

        if (resultSlotUI != null)
        {
            resultSlotUI.SetSlot(currentStation.OutputSlot);
        }
        UpdateResultSlotUI();

        SelectRecipe(null);
        ResetTimerProgress(); // 🛠️ 창이 처음 열릴 때도 타이머 잔상 확정 리셋
        UpdateCookingProgressUI();
    }

    public void CloseStationUI()
    {
        if (currentStation != null)
        {
            currentStation.OnCookingStateChanged -= UpdateCookingProgressUI;
            currentStation.OnOutputSlotChanged -= UpdateResultSlotUI;
        }

        currentStation = null;
        gameObject.SetActive(false);
    }

    private void GenerateRecipeList()
    {
        foreach (Transform child in recipeListParent) Destroy(child.gameObject);
        if (currentStation == null) return;

        foreach (var recipe in currentStation.AvailableRecipes)
        {
            if (recipe == null) continue;
            var item = Instantiate(recipeItemPrefab, recipeListParent);
            item.Init(recipe, this);
        }
    }

    public void SelectRecipe(CookingRecipe recipe)
    {
        selectedRecipe = recipe;
        currentRequestCount = 1;

        if (recipe == null)
        {
            detailAreaRoot.SetActive(false);
            return;
        }

        detailAreaRoot.SetActive(true);
        if (selectedItemIcon != null) selectedItemIcon.sprite = recipe.OutputItem.Icon;
        if (selectedItemNameText != null) selectedItemNameText.text = recipe.RecipeName;

        UpdateIngredientListUI();
        UpdateQuantityTexts();
    }

    private void UpdateIngredientListUI()
    {
        foreach (var ui in spawnedIngredientUIs)
        {
            if (ui != null) Destroy(ui.gameObject);
        }
        spawnedIngredientUIs.Clear();

        if (selectedRecipe == null || ingredientPrefab == null || ingredientListParent == null) return;

        bool allIngredientsAvailable = true;

        foreach (var ing in selectedRecipe.Ingredients)
        {
            if (ing.RequiredItem == null) continue;

            IngredientItemUI ingUI = Instantiate(ingredientPrefab, ingredientListParent);
            bool hasEnough = ingUI.Init(ing.RequiredItem, ing.RequiredAmount, currentRequestCount, playerInventory);

            if (!hasEnough) allIngredientsAvailable = false;
            spawnedIngredientUIs.Add(ingUI);
        }

        if (currentStation != null && !currentStation.IsCooking)
        {
            cookStartButton.interactable = allIngredientsAvailable;
        }
    }

    private void UpdateQuantityTexts()
    {
        if (requestCountText != null) requestCountText.text = currentRequestCount.ToString();
    }

    public void ChangeQuantity(int amount)
    {
        if (selectedRecipe == null) return;

        currentRequestCount = Mathf.Clamp(currentRequestCount + amount, 1, selectedRecipe.OutputItem.MaximumStacks);

        UpdateQuantityTexts();
        UpdateIngredientListUI();
    }

    public void OnClickStartCook()
    {
        if (currentStation == null || selectedRecipe == null || playerInventory == null) return;
        if (currentStation.IsCooking) return;

        currentStation.StartCooking(selectedRecipe, currentRequestCount, playerInventory);
        UpdateIngredientListUI();
    }

    public void OnClickCancelCook()
    {
        if (currentStation == null || playerInventory == null) return;
        currentStation.CancelCooking(playerInventory);

        ResetTimerProgress(); // 🛠️ 취소 버튼 클릭 시 즉시 타이머 수치 강제 리셋 후 동기화
        UpdateIngredientListUI();
    }

    private void UpdateCookingProgressUI()
    {
        if (currentStation == null) return;

        // 요리 진행 중일 때
        if (currentStation.IsCooking && currentStation.CurrentRecipe != null)
        {
            if (cookingProgressRoot != null && !cookingProgressRoot.activeSelf)
                cookingProgressRoot.SetActive(true);

            if (cookStartButton != null) cookStartButton.interactable = false;
            if (plusButton != null) plusButton.interactable = false;
            if (minusButton != null) minusButton.interactable = false;

            // 실시간 타이머 Fill 배율 계산
            float currentProgress = currentStation.CurrentCookTimer / currentStation.CurrentRecipe.BaseCookTime;

            if (timerFillImage != null)
                timerFillImage.fillAmount = currentProgress; // 🛠️ Image.fillAmount 연동

            if (timerText != null)
                timerText.text = $"{(currentStation.CurrentRecipe.BaseCookTime - currentStation.CurrentCookTimer):F1} 초 남음";

            if (queueStatusText != null)
                queueStatusText.text = $"대기열: {currentStation.RemainingQueueCount}개";
        }
        // 요리가 대기 중이거나 완전히 멈췄을 때
        else
        {
            if (cookingProgressRoot != null && cookingProgressRoot.activeSelf)
                cookingProgressRoot.SetActive(false);

            ResetTimerProgress(); // 🛠️ 상태가 꺼질 때 게이지 및 데이터 완전 초기화

            if (plusButton != null) plusButton.interactable = true;
            if (minusButton != null) minusButton.interactable = true;

            if (selectedRecipe != null)
            {
                UpdateIngredientListUI();
            }
            else
            {
                if (cookStartButton != null) cookStartButton.interactable = false;
            }
        }
    }

    /// <summary>
    /// 🛠️ 추가됨: 타이머 상태를 흔적 없이 0으로 초기화하는 청소 함수
    /// </summary>
    private void ResetTimerProgress()
    {
        if (timerFillImage != null) timerFillImage.fillAmount = 0f;
        if (timerText != null) timerText.text = "0.0 초";
        if (queueStatusText != null) queueStatusText.text = "대기열: 0개";
    }

    private void UpdateResultSlotUI()
    {
        if (currentStation == null || resultSlotUI == null) return;
        resultSlotUI.Slot.UpdateSlot();
    }
}