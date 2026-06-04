using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingStationUI : MonoBehaviour
{
    [Header("Connected Core Logic")]
    [SerializeField] private Inventory playerInventory;
    private CookingStation currentStation;

    [Header(" Recipe List Area")]
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private RecipeListItemUI recipeItemPrefab;

    [Header("Selected Recipe Panel")]
    [SerializeField] private GameObject detailAreaRoot;
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private TMP_Text selectedItemNameText;

    [Header("Ingredient UI Area ")]
    [SerializeField] private Transform ingredientListParent;
    [SerializeField] private IngredientItemUI ingredientPrefab;

    [Header("Quantity Selector Control")]
    [SerializeField] private TMP_Text requestCountText;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button cookStartButton;

    [Header("Cooking Status HUD Zone")]
    [SerializeField] private GameObject cookingProgressRoot;
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

        ResetTimerProgress(); 
        UpdateIngredientListUI();
    }

    private void UpdateCookingProgressUI()
    {
        if (currentStation == null) return;

        if (currentStation.IsCooking && currentStation.CurrentRecipe != null)
        {
            if (cookingProgressRoot != null && !cookingProgressRoot.activeSelf)
                cookingProgressRoot.SetActive(true);

            if (cookStartButton != null) cookStartButton.interactable = false;
            if (plusButton != null) plusButton.interactable = false;
            if (minusButton != null) minusButton.interactable = false;

            float currentProgress = currentStation.CurrentCookTimer / currentStation.CurrentRecipe.BaseCookTime;

            if (timerFillImage != null)
                timerFillImage.fillAmount = currentProgress; 

            if (timerText != null)
                timerText.text = $"{(currentStation.CurrentRecipe.BaseCookTime - currentStation.CurrentCookTimer):F1} 초 남음";

            if (queueStatusText != null)
                queueStatusText.text = $"대기열: {currentStation.RemainingQueueCount}개";
        }
        else
        {
            if (cookingProgressRoot != null && cookingProgressRoot.activeSelf)
                cookingProgressRoot.SetActive(false);

            ResetTimerProgress(); 

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