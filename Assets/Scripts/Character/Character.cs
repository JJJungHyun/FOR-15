using UnityEngine;
using CharacterStats;
using System.Collections.Generic;
using UnityEngine.Profiling;

public class Character : MonoBehaviour
{
    private Dictionary<string, IActivatableDevice> _devices = new Dictionary<string, IActivatableDevice>();
    
    [Header("Initial Base Stats")]
    [SerializeField] private float baseStr = 10f;
    [SerializeField] private float baseDef = 5f;
    [SerializeField] private float baseHp = 100f;
    [SerializeField] private float baseHunger = 100f;

    [Header("Stats Instances")]
    public Stat Strength;
    public Stat Defense;
    public ClampedStat Health;
    public ClampedStat Hunger;

    [Header("Handlers")]
    public CharConditionHandler ConditionHandler;

    [Header("UI")]
    [SerializeField] private StatBar hpBar;
    [SerializeField] private StatBar hungerBar;
    [SerializeField] private StatPanel statPanel;
    [SerializeField] private CharProfileUI profileUI;


    // --- 추가된 변수 ---
    private bool _isDead = false;

    private void Awake()
    {
        Strength = new Stat(baseStr);
        Defense = new Stat(baseDef);
        Health = new ClampedStat(baseHp);
        Hunger = new ClampedStat(baseHunger);

        ConditionHandler = GetComponent<CharConditionHandler>();
        if (ConditionHandler != null)
            ConditionHandler.Init(this);

        RefreshDeviceList();
    }

    private void Start()
    {
        if (statPanel != null)
            statPanel.SetStats(Strength, Defense);

        if (hpBar != null) hpBar.Bind(Health);
        if (hungerBar != null) hungerBar.Bind(Hunger);
        if (profileUI != null) profileUI.Bind(Health);
    }

    public void RefreshDeviceList()
    {
        var foundDevices = GetComponentsInChildren<IActivatableDevice>(true);
        foreach (var device in foundDevices)
        {
            if (!_devices.ContainsKey(device.DeviceID))
            {
                _devices[device.DeviceID] = device;
            }
        }
    }

    public bool TryGetDevice(string deviceID, out IActivatableDevice device)
    {
        return _devices.TryGetValue(deviceID, out device);
    }

    public void RegisterDevice(string deviceID, IActivatableDevice device)
    {
        if (!_devices.ContainsKey(deviceID))
        {
            _devices.Add(deviceID, device);
        }
    }

    public void UseDevice(string deviceID)
    {
        if (_devices.TryGetValue(deviceID, out var device))
        {
            device.Activate();
        }
    }

    // --- 추가된 함수: 외부(몬스터 등)에서 데미지를 줄 때 호출 ---
    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        // Value가 아니라 CurrentValue를 수정해야 합니다!
        Health.CurrentValue -= damage;

        Debug.Log($"현재 체력: {Health.CurrentValue}");

        if (Health.CurrentValue <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        Health.CurrentValue = 0;

        Debug.Log("플레이어가 사망했습니다.");

        // 이전에 만든 GameOverManager 호출
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.StartGameOver();
        }
    }

    public float GetAttackDamage() => Strength.Value;

    public void LoadFromData(GameData data)
    {
        // 1. 공격력, 방어력 복구
        if (Strength != null) Strength.BaseValue = data.strength;
        if (Defense != null) Defense.BaseValue = data.defense;

        // 2. 체력 복구 (최대치 설정 후 현재치 설정)
        if (Health != null)
        {
            Health.BaseValue = data.maxHp;
            Health.CurrentValue = data.currentHp;
        }

        // 3. 허기 복구
        if (Hunger != null)
        {
            Hunger.BaseValue = data.maxHunger;
            Hunger.CurrentValue = data.currentHunger;
        }

        // 4. UI가 있다면 업데이트
        if (statPanel != null) statPanel.SetStats(Strength, Defense);

        Debug.Log("캐릭터 스탯 로드 완료!");
    }
}