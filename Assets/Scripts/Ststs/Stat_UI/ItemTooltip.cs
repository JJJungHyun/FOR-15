using TMPro;
using UnityEngine;

public class ItemTooltip : Tooltip
{
    public static ItemTooltip Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowTooltip(Item item)
    {
        if (item == null) return;

        itemNameText.text = item.ItemName;
        itemTypeText.text = $"<color=#CCCCCC>[{item.GetItemType()}]</color>";
        itemDescriptionText.text = item.GetDescription();

        base.ShowTooltip();
    }
}