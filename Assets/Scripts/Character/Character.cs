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

    public float GetAttackDamage() => Strength.Value;
}