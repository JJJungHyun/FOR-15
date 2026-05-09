using UnityEngine;

public class RangedAbility : MonoBehaviour, IWeaponAbility
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private float minRange = 2f;
    [SerializeField] private float maxRange = 12f;
    [SerializeField] private float projectileSpeed = 15f;

    private float currentChargeTime;
    private bool isCharging;

    public bool IsAttacking => isCharging;

    public float ChargeRatio => Mathf.Clamp01(currentChargeTime / maxChargeTime);

    public void OnAttackStart(Character player)
    {
        isCharging = true;
        currentChargeTime = 0f;
    }

    public void OnAttackHold(Character player)
    {
        if (!isCharging) return;
        currentChargeTime += Time.deltaTime;
    }

    public void OnAttackRelease(Character player)
    {
        if (!isCharging) return;
        Fire(player);
        isCharging = false;
    }

    private void Fire(Character player)
    {
        float finalRange = Mathf.Lerp(minRange, maxRange, ChargeRatio);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 dir = (mousePos - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        if (proj.TryGetComponent(out Projectile projectileScript))
        {
            projectileScript.Setup(dir, projectileSpeed, finalRange, player.GetAttackDamage());
        }
    }
}