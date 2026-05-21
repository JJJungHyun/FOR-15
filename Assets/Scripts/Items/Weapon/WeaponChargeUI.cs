using UnityEngine;
using UnityEngine.UI;

public class WeaponChargeUI : MonoBehaviour
{
    [SerializeField] private PlayerEquipment playerEquipment;
    [SerializeField] private GameObject chargeBarObject;
    [SerializeField] private Image chargeFillImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color fullChargeColor = Color.green;

    private void Update()
    {
        if (playerEquipment == null) return;

        var ability = playerEquipment.CurrentWeaponAbility;

        RangedAbility rangedAbility = ability as RangedAbility;

        if (rangedAbility != null && rangedAbility.IsAttacking)
        {
            if (chargeBarObject != null) chargeBarObject.SetActive(true);

            float ratio = rangedAbility.ChargeRatio;
            chargeFillImage.fillAmount = ratio;
            chargeFillImage.color = (ratio >= 1.0f) ? fullChargeColor : normalColor;
        }
        else
        {
            if (chargeBarObject != null) chargeBarObject.SetActive(false);
        }
    }
}