using CharacterStats;
using System;
using UnityEngine; // Mathf 사용을 위해 추가

namespace CharacterStats
{
    public class ClampedStat : Stat
    {
        private float _maxValue;
        private float _currentValue;

        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                // Math.Clamp 또는 Mathf.Clamp를 사용하여 0 ~ 최대값 사이로 제한
                _currentValue = Mathf.Clamp(value, 0, _maxValue);
                OnCurrentValueChanged?.Invoke(_currentValue);
            }
        }

        public event Action<float> OnCurrentValueChanged;

        public ClampedStat(float maxValue) : base(maxValue)
        {
            _maxValue = maxValue;     // [추가] 이 줄이 없으면 _maxValue가 0이 됩니다!
            _currentValue = maxValue;
        }
    }
}