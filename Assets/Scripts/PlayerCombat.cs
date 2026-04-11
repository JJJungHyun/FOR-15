using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Character player;
    private PlayerEquipment equipment;

    [Header("Visual Settings")]
    [SerializeField] private float holderOffset = 0.5f;

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
        if (equipment.CurrentWeaponAbility != null)
        {
            equipment.CurrentWeaponAbility.ExecuteAttack(player);
        }
    }

    private void HandleWeaponFacing()
    {
        Transform holder = equipment.WeaponHolder;
        if (holder == null || Camera.main == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - transform.position).normalized;

        holder.rotation = Quaternion.identity;

        Vector3 parentScale = transform.localScale;
        holder.localScale = new Vector3(
            1f / Mathf.Max(0.01f, Mathf.Abs(parentScale.x)),
            1f / Mathf.Max(0.01f, Mathf.Abs(parentScale.y)),
            1f / Mathf.Max(0.01f, Mathf.Abs(parentScale.z))
        );

        float flipX = (direction.x > 0) ? -1f : 1f;
        holder.localPosition = new Vector3((direction.x > 0) ? holderOffset : -holderOffset, 0, 0);

        if (equipment.CurrentWeaponInstance != null)
        {
            equipment.CurrentWeaponInstance.transform.localScale = new Vector3(flipX, 1, 1);
            AlwaysShowWeaponOnTop();
        }
    }

    private void AlwaysShowWeaponOnTop()
    {
        SpriteRenderer weaponSr = equipment.CurrentWeaponInstance.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSr = GetComponent<SpriteRenderer>();
        if (weaponSr != null && playerSr != null)
            weaponSr.sortingOrder = playerSr.sortingOrder + 1;
    }
}