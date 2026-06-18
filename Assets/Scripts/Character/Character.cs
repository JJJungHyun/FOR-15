using CharacterStats;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Death")]
    [SerializeField] private string fallbackTitleSceneName = "TitleScene";

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
    public bool IsDead => _isDead;

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

        CheckDeath();
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

    private IEnumerator ResetFarmingState(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsFarming = false;
    }

    public void TakeDamage(float damage, Vector2 attackerPosition)
    {
        bool tookDamage = ApplyDamage(damage, true);
        if (tookDamage && ConditionHandler != null)
        {
            ConditionHandler.ApplyKnockback(attackerPosition);
        }
    }

    public void TakeDamage(float damage)
    {
        ApplyDamage(damage, true);
    }

    public void TakeDirectDamage(float damage)
    {
        ApplyDamage(damage, false);
    }
    public void Heal(float amount)
    {
        if (_isDead || Health == null) return;
        Health.CurrentValue += Mathf.Max(0f, amount);
    }

    public void CheckDeath()
    {
        if (!_isDead && Health != null && Health.CurrentValue <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;
        IsFarming = false;

        if (_farmingResetCoroutine != null)
        {
            StopCoroutine(_farmingResetCoroutine);
            _farmingResetCoroutine = null;
        }

        if (Health != null) Health.CurrentValue = 0f;

        DisablePlayerControl();
        StartGameOver();
    }

    public float GetAttackDamage() => Strength.Value;

    public void LoadFromData(GameData data)
    {
        _isDead = false;

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

        CheckDeath();
    }

    private bool ApplyDamage(float damage, bool reduceByDefense)
    {
        if (_isDead || Health == null) return false;

        float finalDamage = Mathf.Max(0f, damage);
        if (reduceByDefense && Defense != null)
        {
            finalDamage = Mathf.Max(0f, finalDamage - Defense.Value);
        }

        if (finalDamage <= 0f) return false;

        Health.CurrentValue -= finalDamage;
        Debug.Log($"[Player] Damaged: {finalDamage}, HP: {Health.CurrentValue}");

        CheckDeath();
        return true;
    }

    private void DisablePlayerControl()
    {
        if (TryGetComponent(out PlayerInputHandler inputHandler)) inputHandler.enabled = false;
        if (TryGetComponent(out PlayerControl playerControl)) playerControl.enabled = false;
        if (TryGetComponent(out PlayerCombat playerCombat)) playerCombat.enabled = false;
        if (TryGetComponent(out PlayerInteraction playerInteraction)) playerInteraction.enabled = false;

        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void StartGameOver()
    {
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.StartGameOver();
            return;
        }

        Debug.LogWarning("GameOverManager was not found. Loading the title scene directly.");
        SceneManager.LoadScene(fallbackTitleSceneName);
    }
}