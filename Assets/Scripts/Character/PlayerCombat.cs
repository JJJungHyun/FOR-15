using Unity.AppUI.Core;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Character player;
    private PlayerEquipment equipment;
    private PlayerAnimation playerAnimation;
    private SpriteRenderer spriteRenderer;

    [Header("Default Settings")]
    [SerializeField] private MeleeAbility bareHandAbility;

    private float currentChargeTime;
    private bool isCharging = false;
    private float maxChargeTime = 1.5f;
    private float lastAttackTime = -99f;

    [SerializeField] private GameObject handEffectPrefab;  // 맨손용 이펙트 추가
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private Transform slashEffectPoint;
    [SerializeField] private float slashEffectLifeTime = 0.25f;

    public bool IsCharging => isCharging;
    public float ChargeRatio => isCharging ? Mathf.Clamp01(currentChargeTime / maxChargeTime) : 0f;

    private void Awake()
    {
        player = GetComponent<Character>();
        equipment = GetComponent<PlayerEquipment>();
        playerAnimation = new PlayerAnimation(GetComponent<Animator>());
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (currentChargeTime < maxChargeTime)
            {
                currentChargeTime += Time.deltaTime;
                if (currentChargeTime > maxChargeTime)
                {
                    currentChargeTime = maxChargeTime;
                }
            }
        }
    }
    private void SpawnSlashEffect(Vector3 dir, GameObject effectPrefab)
    {
        if (effectPrefab == null) return; // 기존 slashEffectPrefab 대신 매개변수 검사
        if (dir.sqrMagnitude <= 0.01f) return;

        dir.Normalize();
        Vector3 spawnOffset = Vector3.zero;
        float angle = 0f;
        Vector3 scale = Vector3.one;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // 좌우 공격
            if (dir.x > 0f)
            {
                // 오른쪽
                spawnOffset = new Vector3(2f, 0f, 0f);
                angle = 180f;
                scale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                // 왼쪽
                spawnOffset = new Vector3(-2f, 0f, 0f);
                angle = 0f;
                scale = new Vector3(-1f, 1f, 1f);
            }
        }
        else
        {
            if (dir.y > 0f)
            {
                // 위쪽 공격
                spawnOffset = new Vector3(0f, 2f, 0f);
                angle = -90f;
                scale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                // 아래쪽 공격
                spawnOffset = new Vector3(0f, -2f, 0f);
                angle = 90f;
                scale = new Vector3(1f, 1f, 1f);
            }
        }

        GameObject effect = Instantiate(
             effectPrefab,
             transform.position + spawnOffset,
             Quaternion.Euler(0f, 0f, angle)
         );

        effect.transform.localScale = scale;
        Destroy(effect, slashEffectLifeTime);
    }

    private void HandleAttackStart()
    {
        IWeaponAbility ability = GetCurrentAbility();

        if (ability == null) return;

        float cooldown = (ability is MeleeAbility melee) ? melee.AttackCooldown :
                         (ability is RangedAbility ranged) ? ranged.AttackCooldown : 0.4f;

        if (Time.time < lastAttackTime + cooldown) return;

        Vector3 mouseDir = GetMouseDirection();

        if (ability is RangedAbility rangedAbility)
        {
            if (!rangedAbility.HasAmmo(player))
            {
                return;
            }

            isCharging = true;
            currentChargeTime = 0f;
            maxChargeTime = rangedAbility.MaxChargeTime;

            ability.OnAttackStart(player, mouseDir);
        }
        else
        {
            lastAttackTime = Time.time;

            ability.OnAttackStart(player, mouseDir);

            bool isBareHand = ((object)ability == bareHandAbility);

            if (Mathf.Abs(mouseDir.x) > Mathf.Abs(mouseDir.y))
            {
                spriteRenderer.flipX = (mouseDir.x > 0f);

                if (isBareHand)
                {
                    // 맨손 좌우 공격
                    playerAnimation.PlayAnimState(PlayerAnimState.HandLAttack);
                    SpawnSlashEffect(mouseDir, handEffectPrefab);
                }
                else
                {
                    // 무기 좌우 공격
                    playerAnimation.PlayAnimState(PlayerAnimState.WeaponLAttack);
                    SpawnSlashEffect(mouseDir, slashEffectPrefab);
                }
            }
            else
            {
                if (mouseDir.y < 0f)
                {
                    if (isBareHand)
                    {
                        // 맨손 아래 공격
                        playerAnimation.PlayAnimState(PlayerAnimState.HandDAttack);
                        SpawnSlashEffect(mouseDir, handEffectPrefab);
                    }
                    else
                    {
                        // 무기 아래 공격
                        playerAnimation.PlayAnimState(PlayerAnimState.WeaponDAttack);
                        SpawnSlashEffect(mouseDir, slashEffectPrefab);
                    }
                }
                else
                {
                    if (isBareHand)
                    {
                        // 맨손 위 공격
                        playerAnimation.PlayAnimState(PlayerAnimState.HandUAttack);
                        SpawnSlashEffect(mouseDir, handEffectPrefab);
                    }
                    else
                    {
                        // 무기 위 공격
                        playerAnimation.PlayAnimState(PlayerAnimState.WeaponUAttack);
                        SpawnSlashEffect(mouseDir, slashEffectPrefab);
                    }
                }
            }
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