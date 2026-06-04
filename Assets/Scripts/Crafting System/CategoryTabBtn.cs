using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CategoryTabBtn : MonoBehaviour
{
    [Header("Sprites")]
    [Tooltip("탭이 선택되었을 때 켜질 갈색/밝은 이미지")]
    [SerializeField] private Sprite selectedSprite;
    [Tooltip("탭이 비활성화 상태일 때의 어두운 이미지")]
    [SerializeField] private Sprite unselectedSprite;

    private Image targetImage;
    private Button button;
    private CraftingPanel panel;
    private int categoryIndex;

    public void Init(CraftingPanel craftingPanel, int index)
    {
        panel = craftingPanel;
        categoryIndex = index;

        targetImage = GetComponent<Image>();
        button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => panel.SetCategory(categoryIndex));
    }

    public void SetSelectState(bool isSelected)
    {
        if (targetImage == null) return;

        targetImage.sprite = isSelected ? selectedSprite : unselectedSprite;

        // 선택된 탭은 약간 강조되도록 처리 가능
        targetImage.color = isSelected ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
    }
}