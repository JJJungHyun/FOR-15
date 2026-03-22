using CharacterStats;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI valueText;

    private Stat _stat;
    private string _statName;

    public void Init(Stat stat, string statName)
    {
        if (_stat != null) _stat.OnValueChanged -= UpdateStatValue;

        _stat = stat;
        _statName = statName;

        nameText.text = _statName;
        _stat.OnValueChanged += UpdateStatValue;
        UpdateStatValue(_stat);
    }

    private void UpdateStatValue(Stat stat)
    {
        valueText.text = stat.Value.ToString("N0");

        if (StatTooltip.Instance != null && StatTooltip.Instance.gameObject.activeSelf)
            StatTooltip.Instance.ShowTooltip(_stat, _statName);
    }

    public void OnPointerEnter(PointerEventData eventData) => StatTooltip.Instance?.ShowTooltip(_stat, _statName);
    public void OnPointerExit(PointerEventData eventData) => StatTooltip.Instance?.HideTooltip();

    private void OnDestroy()
    {
        if (_stat != null) _stat.OnValueChanged -= UpdateStatValue;
    }
}