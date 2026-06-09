using UnityEngine;
using UnityEngine.EventSystems;

public class CharPanelMgr : MonoBehaviour
{
    public static CharPanelMgr Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainGroup;

    [Header("References in MainGroup")]
    [SerializeField] private CraftingPanel craftingPanel;
    [SerializeField] private InventoryController inventoryController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        if (mainGroup != null)
        {
            mainGroup.SetActive(false);
        }
        UpdateGameState(false);
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnToggleCombinedUI += ToggleCombinedUI;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnToggleCombinedUI -= ToggleCombinedUI;
    }

    public void ToggleCombinedUI()
    {
        if (mainGroup == null) return;

        if (mainGroup.activeSelf)
        {
            CloseAll();
        }
        else
        {
            OpenCombinedUI();
        }
    }

    public void OpenCombinedUI()
    {
        if (mainGroup == null) return;

        mainGroup.SetActive(true);

        if (craftingPanel != null)
        {
            craftingPanel.RefreshRecipeList();
        }
        else
        {
            GetComponentInChildren<CraftingPanel>(true)?.RefreshRecipeList();
        }

        if (inventoryController != null)
        {
            inventoryController.RefreshAllSlots();
        }
        else
        {
            GetComponentInChildren<InventoryController>(true)?.RefreshAllSlots();
        }

        UpdateGameState(true);
    }

    public void CloseAll()
    {
        if (mainGroup != null)
        {
            mainGroup.SetActive(false);
        }
        UpdateGameState(false);
    }

    private void UpdateGameState(bool isOpen)
    {
        Tooltip.ResetTooltipState();
        ItemTooltip.Instance?.HideTooltip();
        StatTooltip.Instance?.HideTooltip();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}