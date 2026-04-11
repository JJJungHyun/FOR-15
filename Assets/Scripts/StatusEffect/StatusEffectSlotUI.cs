using UnityEngine;
using UnityEngine.UI;

public class StatusEffectSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image fillImage;

    private ConditionEffect targetEffect;
    private float currentMaxDuration; 

    public string EffectName => targetEffect?.Name;

    public void Setup(ConditionEffect effect)
    {
        targetEffect = effect;
        currentMaxDuration = effect.Duration;
    }

    private void Update()
    {
        if (targetEffect == null || targetEffect.Duration <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (targetEffect.Duration > currentMaxDuration)
        {
            currentMaxDuration = targetEffect.Duration;
        }

        fillImage.fillAmount = targetEffect.Duration / currentMaxDuration;
    }
}