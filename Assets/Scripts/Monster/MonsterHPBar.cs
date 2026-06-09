using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    [SerializeField] private Image hpFillImage;
    [SerializeField] private bool hideOnFullHp = true;

    private Transform monsterTransform;
    private float maxHp;

    public void Init(float maxHp, Transform owner)
    {
        monsterTransform = owner;
        this.maxHp = maxHp;

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = 1f;
        }
    }

    public void UpdateHP(float currentHp)
    {
        if (hpFillImage == null) return;

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        hpFillImage.fillAmount = Mathf.Clamp01(currentHp / maxHp);

        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (monsterTransform != null)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}