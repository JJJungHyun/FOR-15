using UnityEngine;
using UnityEngine.EventSystems;

public class CharPanelMgr : MonoBehaviour
{
    public static CharPanelMgr Instance { get; private set; }

    [Header("Panels")]
    [Tooltip("인벤토리와 제작창이 한 이미지에 합쳐진 최상위 UI 패널 오브젝트")]
    [SerializeField] private GameObject mainGroup;

    [Header("References in MainGroup")]
    [SerializeField] private CraftingPanel craftingPanel;
    [SerializeField] private InventoryController inventoryController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"씬에 CharPanelMgr가 중복 배치되어 {gameObject.name}의 컴포넌트를 파괴합니다.");
            Destroy(this);
            return;
        }

        Instance = this;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Start()
    {
        // 최상위 UIManager 오브젝트가 꺼지는 걸 방지하기 위해 mainGroup만 안전하게 비활성화
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

        // 1. 제작창 레시피 리프레시
        if (craftingPanel != null)
        {
            craftingPanel.RefreshRecipeList();
        }
        else
        {
            // 수동 할당 안 되어있으면 자식에서 찾아서 리프레시
            GetComponentInChildren<CraftingPanel>(true)?.RefreshRecipeList();
        }

        // 2. 인벤토리 슬롯 UI 리프레시 연동
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