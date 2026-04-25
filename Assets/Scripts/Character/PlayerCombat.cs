using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Character player;
    private PlayerEquipment equipment;

    [Header("Visual Settings")]
    [SerializeField] private float offsetX = 0.5f;
    [SerializeField] private float offsetY = 0.5f;
    [SerializeField] private float weaponVisualScale = 1.0f;

    private void Awake()
    {
        player = GetComponent<Character>();
        equipment = GetComponent<PlayerEquipment>();
    }

    private void OnEnable() => PlayerInputHandler.OnAttackPressed += TryAttack;
    private void OnDisable() => PlayerInputHandler.OnAttackPressed -= TryAttack;

    private void Update() => HandleWeaponFacing();

    public void TryAttack()
    {
        if (equipment != null && equipment.CurrentWeaponAbility != null)
        {
            equipment.CurrentWeaponAbility.ExecuteAttack(player);
        }
    }

    private void HandleWeaponFacing()
    {
        Transform holder = equipment.WeaponHolder;
        if (holder == null || Camera.main == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 direction = (mousePos - transform.position).normalized;

        bool isRight = direction.x > 0;
        float targetX = isRight ? offsetX : -offsetX;
        holder.localPosition = new Vector3(targetX, offsetY, 0f);

        holder.localRotation = Quaternion.identity;
        holder.localScale = Vector3.one;

        if (equipment.CurrentWeaponInstance != null)
        {
            float flipX = isRight ? -weaponVisualScale : weaponVisualScale;
            equipment.CurrentWeaponInstance.transform.localScale = new Vector3(flipX, weaponVisualScale, 1f);
            AlwaysShowWeaponOnTop();
        }
    }

    private void AlwaysShowWeaponOnTop()
    {
        if (equipment.CurrentWeaponInstance == null) return;

        SpriteRenderer weaponSr = equipment.CurrentWeaponInstance.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSr = GetComponent<SpriteRenderer>();

        if (weaponSr != null && playerSr != null)
        {
            weaponSr.sortingOrder = playerSr.sortingOrder + 1;
        }
    }
}