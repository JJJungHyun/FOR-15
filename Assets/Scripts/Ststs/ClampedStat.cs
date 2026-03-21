using CharacterStats;
using System;

namespace CharacterStats
{
    // 최대값이 정해져 있고 현재값이 그 안에서 움직이는 스탯
    public class ClampedStat : Stat
    {
        private float _currentValue;
        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = Math.Clamp(value, 0, Value);
                OnCurrentValueChanged?.Invoke(_currentValue);
            }
        }

        public event Action<float> OnCurrentValueChanged;

        public ClampedStat(float maxValue) : base(maxValue)
        {
            _currentValue = maxValue;
        }
    }
}