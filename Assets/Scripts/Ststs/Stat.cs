using CharacterStats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CharacterStats
{
    [Serializable]
    public class Stat
    {
        public float BaseValue;

        public event Action<Stat> OnValueChanged;

        protected bool isDirty = true;
        protected float lastBaseValue;
        protected float _value;

        public virtual float Value
        {
            get
            {
                if (isDirty || Math.Abs(lastBaseValue - BaseValue) > 0.0001f)
                {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            }
        }

        protected readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        public Stat()
        {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public Stat(float baseValue) : this() => BaseValue = baseValue;

        public virtual void AddModifier(StatModifier mod)
        {
            isDirty = true;
            statModifiers.Add(mod);
            statModifiers.Sort(CompareModifierOrder);
            OnValueChanged?.Invoke(this); 
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (statModifiers.Remove(mod))
            {
                isDirty = true;
                OnValueChanged?.Invoke(this); 
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            int numRemoved = statModifiers.RemoveAll(mod => mod.Source == source);

            if (numRemoved > 0)
            {
                isDirty = true;
                OnValueChanged?.Invoke(this);
                return true;
            }
            return false;
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order) return -1;
            if (a.Order > b.Order) return 1;
            return 0;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;

                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.Type == StatModType.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
            }

            return (float)Math.Round(finalValue, 4);
        }
    }
}