using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Character player;
    private PlayerEquipment equipment;

    [Header("Default Settings")]
    [SerializeField] private MeleeAbility bareHandAbility; 

    private float currentChargeTime;
    private bool isCharging = false;
    private float maxChargeTime = 1.5f;
    private float lastAttackTime = -99f; 

    public bool IsCharging => isCharging;
    public float ChargeRatio => isCharging ? Mathf.Clamp01(currentChargeTime / maxChargeTime) : 0f;

    private void Awake()
    {
        player = GetComponent<Character>();
        equipment = GetComponent<PlayerEquipment>();
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnAttackPressed += HandleAttackStart;
        PlayerInputHandler.OnAttackHeld += HandleAttackHold;
        PlayerInputHandler.OnAttackReleased += HandleAttackRelease;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnAttackPressed -= HandleAttackStart;
        PlayerInputHandler.OnAttackHeld -= HandleAttackHold;
        PlayerInputHandler.OnAttackReleased -= HandleAttackRelease;
    }

    private void Update()
    {
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
        }
    }

    private void HandleAttackStart()
    {
        IWeaponAbility ability = GetCurrentAbility();
        if (ability == null) return;

        float cooldown = (ability is MeleeAbility melee) ? melee.AttackCooldown :
                         (ability is RangedAbility ranged) ? ranged.AttackCooldown : 0.4f;

        if (Time.time < lastAttackTime + cooldown) return;

        if (ability is RangedAbility rangedAbility)
        {
            isCharging = true;
            currentChargeTime = 0f;
            maxChargeTime = rangedAbility.MaxChargeTime;

            ability.OnAttackStart(player, GetMouseDirection());
        }
        else
        {
            lastAttackTime = Time.time;
            ability.OnAttackStart(player, GetMouseDirection());
        }
    }

    private void HandleAttackHold() { }

    private void HandleAttackRelease()
    {
        if (!isCharging) return;

        IWeaponAbility ability = GetCurrentAbility();
        if (ability != null && ability is RangedAbility)
        {
            lastAttackTime = Time.time;
            ability.OnAttackRelease(player, GetMouseDirection());
        }

        isCharging = false;
    }

    private IWeaponAbility GetCurrentAbility()
    {
        if (equipment == null || equipment.CurrentSelectedWeapon == null)
            return bareHandAbility;

        return equipment.CurrentSelectedWeapon.WeaponAbility;
    }

    private Vector3 GetMouseDirection()
    {
        Camera activeCam = Camera.main;
        if (activeCam == null)
        {
            activeCam = FindFirstObjectByType<Camera>();
            if (activeCam == null) return Vector3.down; 
        }

        Plane xyPlane = new Plane(Vector3.forward, Vector3.zero);

        Ray ray = activeCam.ScreenPointToRay(Input.mousePosition);

        if (xyPlane.Raycast(ray, out float enter))
        {
            Vector3 mouseWorldPos = ray.GetPoint(enter);
            mouseWorldPos.z = 0f; 

            Vector3 direction = mouseWorldPos - transform.position;

            if (direction.sqrMagnitude < 0.001f)
            {
                return Vector3.down;
            }

            return direction.normalized;
        }

        Vector3 fallbackPos = activeCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(activeCam.transform.position.z)));
        fallbackPos.z = 0f;
        return (fallbackPos - transform.position).normalized;
    }
}