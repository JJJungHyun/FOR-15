using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterStats;

public class StatBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image fillImage; 
    [SerializeField] private TextMeshProUGUI valueText; 
    [SerializeField] private string statLabel;      

    private ClampedStat _stat;

    public void Bind(ClampedStat stat)
    {
        if (_stat != null)
        {
            _stat.OnCurrentValueChanged -= UpdateBar;
            _stat.OnValueChanged -= OnMaxStatChanged;
        }

        _stat = stat;
        _stat.OnCurrentValueChanged += UpdateBar; 
        _stat.OnValueChanged += OnMaxStatChanged;  

        RefreshUI();
    }

    private void OnMaxStatChanged(Stat stat) => RefreshUI();

    private void UpdateBar(float currentValue) => RefreshUI();

    private void RefreshUI()
    {
        if (_stat == null || fillImage == null) return;

        float ratio = _stat.CurrentValue / _stat.Value;
        fillImage.fillAmount = ratio;

        if (valueText != null)
        {
            valueText.text = $"{statLabel} : {_stat.CurrentValue:0} / {_stat.Value:0}";
        }
    }

    private void OnDestroy()
    {
        if (_stat != null)
        {
            _stat.OnCurrentValueChanged -= UpdateBar;
            _stat.OnValueChanged -= OnMaxStatChanged;
        }
    }
}