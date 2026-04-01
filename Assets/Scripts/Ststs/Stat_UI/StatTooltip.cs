using System.Text;
using UnityEngine;
using TMPro;
using CharacterStats;

public class StatTooltip : Tooltip
{
    public static StatTooltip Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statModifiersText;

    private readonly StringBuilder sb = new StringBuilder();

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameObject.SetActive(false);
    }

    public void ShowTooltip(Stat stat, string statName)
    {
        statNameText.text = BuildTopText(stat, statName);
        statModifiersText.text = BuildModifiersText(stat);
        base.ShowTooltip();
    }

    private string BuildTopText(Stat stat, string statName)
    {
        sb.Clear();
        sb.Append($"<color=#FFD700>{statName}</color> {stat.Value}");

        if (Mathf.Abs(stat.Value - stat.BaseValue) > 0.01f)
        {
            float diff = (float)System.Math.Round(stat.Value - stat.BaseValue, 2);
            string color = diff > 0 ? "#00FF00" : "#FF0000";
            sb.Append($" (<color={color}>{(diff > 0 ? "+" : "")}{diff}</color>)");
        }
        return sb.ToString();
    }

    private string BuildModifiersText(Stat stat)
    {
        sb.Clear();
        foreach (var mod in stat.StatModifiers)
        {
            if (sb.Length > 0) sb.AppendLine();

            string sign = mod.Value > 0 ? "+" : "";
            string value = mod.Type == StatModType.Flat ? $"{mod.Value}" : $"{mod.Value * 100}%";
            string sourceName = (mod.Source is Item item) ? item.ItemName : "Unknown";

            sb.Append($"{sign}{value} <size=80%>[{sourceName}]</size>");
        }
        return sb.Length > 0 ? sb.ToString() : "";
    }
}