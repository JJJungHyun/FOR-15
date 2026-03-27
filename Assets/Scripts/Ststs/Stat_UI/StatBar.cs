using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CharacterStats;

public class StatBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image fillImage;        // 슬라이더의 Fill 부분
    [SerializeField] private TextMeshProUGUI valueText; // "85 / 100" 형태의 텍스트
    [SerializeField] private string statLabel;       // "HP", "Hunger" 등

    private ClampedStat _stat;

    // 초기 연결 (StatPanel이나 Character에서 호출)
    public void Bind(ClampedStat stat)
    {
        if (_stat != null)
        {
            _stat.OnCurrentValueChanged -= UpdateBar;
            _stat.OnValueChanged -= OnMaxStatChanged;
        }

        _stat = stat;
        _stat.OnCurrentValueChanged += UpdateBar; // 현재값 변경 시 갱신
        _stat.OnValueChanged += OnMaxStatChanged;    // 최대값(장비 등) 변경 시 갱신

        RefreshUI();
    }

    private void OnMaxStatChanged(Stat stat) => RefreshUI();

    private void UpdateBar(float currentValue) => RefreshUI();

    private void RefreshUI()
    {
        if (_stat == null || fillImage == null) return;

        // 1. 비율 계산 (0 ~ 1)
        float ratio = _stat.CurrentValue / _stat.Value;
        fillImage.fillAmount = ratio;

        // 2. 텍스트 갱신 (선택 사항)
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