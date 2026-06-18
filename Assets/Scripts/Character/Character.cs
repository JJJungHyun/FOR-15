using UnityEngine;
using CharacterStats;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour, IDamageable
{
    private Coroutine _farmingResetCoroutine;

    [Header("Inventory Settings")]
    [SerializeField] private ItemContainer playerInventory;

    [Header("Initial Base Stats")]
    [SerializeField] private float baseStr = 10f;
    [SerializeField] private float baseDef = 5f;
    [SerializeField] private float baseHp = 100f;
    [SerializeField] private float baseHunger = 100f;

    public Stat Strength;
    public Stat Defense;
    public ClampedStat Health;
    public ClampedStat Hunger;

    public CharConditionHandler ConditionHandler;

    [SerializeField] private StatBar hpBar;
    [SerializeField] private StatBar hungerBar;
    [SerializeField] private StatPanel statPanel;
    [SerializeField] private CharProfileUI profileUI;

    public bool IsFarming { get; set; } = false;
    private bool _isDead = false;

    private void Awake()
    {
        Strength = new Stat(baseStr);
        Defense = new Stat(baseDef);
        Health = new ClampedStat(baseHp);
        Hunger = new ClampedStat(baseHunger);

        if (ConditionHandler == null) ConditionHandler = GetComponent<CharConditionHandler>();
        if (ConditionHandler != null) ConditionHandler.Init(this);
    }

    private void Start()
    {
        if (statPanel != null) statPanel.SetStats(Strength, Defense);
        if (hpBar != null) hpBar.Bind(Health);
        if (hungerBar != null) hungerBar.Bind(Hunger);
        if (profileUI != null) profileUI.Bind(Health);
    }

    private void OnEnable()
    {
        PlayerInputHandler.OnFarmingPressed += StartFarmingState;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnFarmingPressed -= StartFarmingState;
    }

    private void StartFarmingState()
    {
        if (_isDead) return;
        IsFarming = true;
        if (_farmingResetCoroutine != null) StopCoroutine(_farmingResetCoroutine);
        _farmingResetCoroutine = StartCoroutine(ResetFarmingState(1.0f));
    }

    IEnumerator ResetFarmingState(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsFarming = false;
    }

    public void TakeDamage(float damage, Vector2 attackerPosition) => TakeDamage(damage);

    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        Health.CurrentValue -= damage;
        if (GameOverManager.Instance != null) GameOverManager.Instance.StartGameOver();
    }

    private void Die()
    {
        _isDead = true;
        Health.CurrentValue = 0;
        if (GameOverManager.Instance != null) GameOverManager.Instance.StartGameOver();
    }

    public float GetAttackDamage() => Strength.Value;

    public void LoadFromData(GameData data)
    {
        if (Strength != null) Strength.BaseValue = data.strength;
        if (Defense != null) Defense.BaseValue = data.defense;
        if (Health != null)
        {
            Health.BaseValue = data.maxHp;
            Health.CurrentValue = data.currentHp;
        }
        if (Hunger != null)
        {
            Hunger.BaseValue = data.maxHunger;
            Hunger.CurrentValue = data.currentHunger;
        }
        if (statPanel != null) statPanel.SetStats(Strength, Defense);
    }
}