using CharacterStats;
using System;
using UnityEngine; 

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
                _currentValue = Mathf.Clamp(value, 0, _maxValue);
                OnCurrentValueChanged?.Invoke(_currentValue);
            }
        }

        public event Action<float> OnCurrentValueChanged;

        public ClampedStat(float maxValue) : base(maxValue)
        {
            _maxValue = maxValue;    
            _currentValue = maxValue;
        }
    }
}