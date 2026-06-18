using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialGuideManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject guideRoot;
    [SerializeField] private TextMeshProUGUI guideText;

    [Header("Guide Text")]
    [TextArea(3, 8)]
    [SerializeField] private string firstGuideText;
    [TextArea(3, 8)]
    [SerializeField] private string afterFoodGuideText;
    [SerializeField] private bool showFirstGuideOnStart = true;

    [Header("Food Trigger")]
    [SerializeField] private List<Item> advanceFoodItems = new List<Item>();
    [SerializeField]
    private List<string> advanceFoodNames = new List<string>
    {
        "구운 게",
        "구운 코코넛크랩",
        "구운 코코넛 크랩",
        "구운 검은색코코넛크랩",
        "구운 검은색 코코넛 크랩"
    };

    [Header("Scene")]
    [SerializeField] private bool hideWhenSceneChanges = true;
    [SerializeField] private bool destroyWhenSceneChanges = false;

    private string startSceneName;
    private bool advancedToFoodGuide;

    private void Awake()
    {
        startSceneName = SceneManager.GetActiveScene().name;

        if (guideText == null)
        {
            guideText = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (guideRoot == null && guideText != null)
        {
            guideRoot = guideText.transform.parent != null
                ? guideText.transform.parent.gameObject
                : guideText.gameObject;
        }
    }

    private void OnEnable()
    {
        UsableItem.OnUsed += HandleUsableItemUsed;
        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
    }

    private void Start()
    {
        if (showFirstGuideOnStart)
        {
            ShowFirstGuide();
        }
    }

    private void OnDisable()
    {
        UsableItem.OnUsed -= HandleUsableItemUsed;
        SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
    }

    public void ShowFirstGuide()
    {
        advancedToFoodGuide = false;
        ShowGuide(firstGuideText);
    }

    public void ShowAfterFoodGuide()
    {
        advancedToFoodGuide = true;
        ShowGuide(afterFoodGuideText);
    }

    public void HideGuide()
    {
        if (guideRoot != null)
        {
            guideRoot.SetActive(false);
        }

        if (guideText != null)
        {
            guideText.text = string.Empty;
        }
    }

    private void HandleUsableItemUsed(UsableItem usedItem, Character user)
    {
        if (advancedToFoodGuide || usedItem == null) return;
        if (!IsAdvanceFood(usedItem)) return;

        ShowAfterFoodGuide();
    }

    private bool IsAdvanceFood(UsableItem usedItem)
    {
        foreach (Item item in advanceFoodItems)
        {
            if (item == null) continue;
            if (ReferenceEquals(item, usedItem)) return true;
            if (!string.IsNullOrEmpty(item.ID) && item.ID == usedItem.ID) return true;
        }

        string usedName = NormalizeItemName(usedItem.ItemName);
        string assetName = NormalizeItemName(usedItem.name);

        foreach (string foodName in advanceFoodNames)
        {
            string normalizedFoodName = NormalizeItemName(foodName);
            if (string.IsNullOrEmpty(normalizedFoodName)) continue;

            if (usedName == normalizedFoodName || assetName == normalizedFoodName)
            {
                return true;
            }
        }

        return false;
    }

    private void ShowGuide(string text)
    {
        if (guideRoot != null)
        {
            guideRoot.SetActive(true);
        }

        if (guideText != null)
        {
            guideText.text = text;
        }
    }

    private void HandleActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (!hideWhenSceneChanges) return;
        if (newScene.name == startSceneName) return;

        HideGuide();

        if (destroyWhenSceneChanges)
        {
            Destroy(gameObject);
        }
    }

    private static string NormalizeItemName(string itemName)
    {
        return string.IsNullOrWhiteSpace(itemName)
            ? string.Empty
            : itemName.Replace(" ", string.Empty).Trim().ToLowerInvariant();
    }
}