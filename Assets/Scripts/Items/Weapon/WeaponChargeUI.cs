using UnityEngine;
using UnityEngine.UI;

public class WeaponChargeUI : MonoBehaviour
{
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private GameObject chargeBarObject;
    [SerializeField] private Image chargeFillImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color fullChargeColor = Color.green;

    private void Update()
    {
        if (playerCombat == null) return;

        if (playerCombat.IsCharging)
        {
            if (chargeBarObject != null) chargeBarObject.SetActive(true);

            float ratio = playerCombat.ChargeRatio;
            chargeFillImage.fillAmount = ratio;

            chargeFillImage.color = (ratio >= 1.0f) ? fullChargeColor : normalColor;
        }
        else
        {
            if (chargeBarObject != null) chargeBarObject.SetActive(false);
        }
    }
}