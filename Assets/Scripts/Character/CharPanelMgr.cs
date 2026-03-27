using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharPanelMgr : MonoBehaviour
{
    public static CharPanelMgr Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainGroup;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject craftingPanel;

    [Header("Tab UI Visuals")]
    [SerializeField] private Image inventoryTabImage;
    [SerializeField] private Image craftingTabImage;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f, 1f);

    private void Awake()
    {
        Instance = this;
        CloseAll();
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnInventoryPressed += ToggleInventory;
        PlayerInputHandler.OnCraftingPressed += ToggleCrafting;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnInventoryPressed -= ToggleInventory;
        PlayerInputHandler.OnCraftingPressed -= ToggleCrafting;
    }

    public void ToggleInventory()
    {
        if (mainGroup.activeSelf && inventoryPanel.activeSelf) CloseAll();
        else OpenInventory();
    }

    public void ToggleCrafting()
    {
        if (mainGroup.activeSelf && craftingPanel.activeSelf) CloseAll();
        else OpenCrafting();
    }

    public void OpenInventory()
    {
        mainGroup.SetActive(true);
        inventoryPanel.SetActive(true);
        craftingPanel.SetActive(false);

        UpdateTabVisuals(true);
        UpdateGameState(true);
    }

    public void OpenCrafting()
    {
        mainGroup.SetActive(true);
        inventoryPanel.SetActive(false);
        craftingPanel.SetActive(true);

        craftingPanel.GetComponent<CraftingPanel>()?.RefreshRecipeList();

        UpdateTabVisuals(false); 
        UpdateGameState(true);
    }

    public void CloseAll()
    {
        mainGroup.SetActive(false);
        inventoryPanel.SetActive(false);
        craftingPanel.SetActive(false);

        UpdateGameState(false);
    }

    private void UpdateTabVisuals(bool isInventory)
    {
        if (inventoryTabImage != null) inventoryTabImage.color = isInventory ? activeColor : inactiveColor;
        if (craftingTabImage != null) craftingTabImage.color = isInventory ? inactiveColor : activeColor;
    }

    private void UpdateGameState(bool isOpen)
    {
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;

        Tooltip.ResetTooltipState();
        ItemTooltip.Instance?.HideTooltip();
        StatTooltip.Instance?.HideTooltip();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}