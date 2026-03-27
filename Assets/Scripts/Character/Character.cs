using UnityEngine;
using CharacterStats;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    public Stat Strength = new Stat(10);
    public Stat Defense = new Stat(5);
    public ClampedStat Health = new ClampedStat(100);
    public ClampedStat Hunger = new ClampedStat(100);

    [Header("Handlers")]
    public CharConditionHandler ConditionHandler; 

    [Header("UI")]
    [SerializeField] private StatBar hpBar;
    [SerializeField] private StatBar hungerBar;
    [SerializeField] private StatPanel statPanel;

    private void Awake()
    {
        ConditionHandler = GetComponent<CharConditionHandler>();
        ConditionHandler.Init(this);
    }

    private void Start()
    {
        if (statPanel != null)
            statPanel.SetStats(Strength, Defense);

        if (hpBar != null) hpBar.Bind(Health);
        if (hungerBar != null) hungerBar.Bind(Hunger);
    }

    // 이후 추가할 기능
    // 상태 이상 및 버프 관리
    // 피격 및 사망
    // 이후 많아지면 컴포넌트 분리 후 중앙 연결 기능 수행
}