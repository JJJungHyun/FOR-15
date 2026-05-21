using UnityEngine;
using UnityEngine.UI;
using CharacterStats;

public class CharProfileUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image profileImage;

    [Header("Expression Sprites")]
    [SerializeField] private Sprite[] expressionSprites;

    private ClampedStat targetStat;

    public void Bind(ClampedStat stat)
    {
        if (targetStat != null)
            targetStat.OnCurrentValueChanged -= UpdateProfileExpression;

        targetStat = stat;

        if (targetStat != null)
        {
            targetStat.OnCurrentValueChanged += UpdateProfileExpression;
            UpdateProfileExpression(targetStat.CurrentValue);
        }
    }

    private void OnDestroy()
    {
        if (targetStat != null)
            targetStat.OnCurrentValueChanged -= UpdateProfileExpression;
    }

    private void UpdateProfileExpression(float currentValue)
    {
        if (targetStat == null || profileImage == null) return;
        if (expressionSprites == null || expressionSprites.Length < 4)
        {
            Debug.LogWarning("[ProfileUI] 스프라이트 배열이 비어있거나 4개 미만입니다!");
            return;
        }

        float maxVal = targetStat.Value;
        if (maxVal <= 0) maxVal = 100f;

        float healthPercent = currentValue / maxVal;

        int index;
        if (healthPercent > 0.75f) index = 0;
        else if (healthPercent > 0.50f) index = 1;
        else if (healthPercent > 0.25f) index = 2;
        else index = 3;

        profileImage.sprite = expressionSprites[index];
    }
}
