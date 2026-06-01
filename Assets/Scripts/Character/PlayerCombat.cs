using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Character player;
    private PlayerEquipment equipment;

    [Header("Default Settings")]
    [SerializeField] private MeleeAbility bareHandAbility; // 인스펙터에서 맨손 SO 할당 필수!

    // --- 런타임 변수 관리 통합 (버그 원천 차단) ---
    private float currentChargeTime;
    private bool isCharging = false;
    private float maxChargeTime = 1.5f;
    private float lastAttackTime = -99f; // 공격 쿨타임 역추적용

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
        // 원거리 차징 중일 때 시간 실시간 계산 보장
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
        }
    }

    private void HandleAttackStart()
    {
        IWeaponAbility ability = GetCurrentAbility();
        if (ability == null) return;

        // 1. 공통 쿨타임 체크 (ScriptableObject 데이터 참조 오류 방지용 통합 처리)
        float cooldown = (ability is MeleeAbility melee) ? melee.AttackCooldown :
                         (ability is RangedAbility ranged) ? ranged.AttackCooldown : 0.4f;

        if (Time.time < lastAttackTime + cooldown) return;

        // 2. 능력 종류 분기 처리
        if (ability is RangedAbility rangedAbility)
        {
            isCharging = true;
            currentChargeTime = 0f;
            maxChargeTime = rangedAbility.MaxChargeTime;

            // 원거리 차징 시작 단계 작동 (필요시 애니메이션 조준 트리거)
            ability.OnAttackStart(player, GetMouseDirection());
        }
        else
        {
            // 근접 무기 또는 맨손인 경우 즉시 공격 판정 시동
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

    // 빈 퀵슬롯 선택 시 맨손 공격으로 우회해 주는 핵심 구원 코드
    private IWeaponAbility GetCurrentAbility()
    {
        if (equipment == null || equipment.CurrentSelectedWeapon == null)
            return bareHandAbility;

        return equipment.CurrentSelectedWeapon.WeaponAbility;
    }

    private Vector3 GetMouseDirection()
    {
        if (Camera.main == null) return Vector3.down;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        return (mouseWorldPos - transform.position).normalized;
    }
}