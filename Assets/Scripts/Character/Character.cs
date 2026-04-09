using UnityEngine;
using CharacterStats;

public class Character : MonoBehaviour
{
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

    // --- 추가된 변수 ---
    private bool _isDead = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // K키를 누르면 20씩 데미지
        {
            TakeDamage(20f);
        }
    }

    private void Awake()
    {
        Strength = new Stat(baseStr);
        Defense = new Stat(baseDef);
        Health = new ClampedStat(baseHp);
        Hunger = new ClampedStat(baseHunger);

        ConditionHandler = GetComponent<CharConditionHandler>();
        if (ConditionHandler != null)
            ConditionHandler.Init(this);
    }

    private void Start()
    {
        if (statPanel != null)
            statPanel.SetStats(Strength, Defense);

        if (hpBar != null) hpBar.Bind(Health);
        if (hungerBar != null) hungerBar.Bind(Hunger);
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
}