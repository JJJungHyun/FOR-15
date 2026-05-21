using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("몬스터 성향")]
    public MonsterDisposition disposition;
    public AttackStyle attackStyle;

    [Header("기본 스탯")]
    public float maxHp;
    public float walkSpeed;
    public float chaseSpeed; // 도망이나 추적 시 속도
    public float attackDamage;

    [Header("거리 및 범위")]
    public float detectRange; // 플레이어 인지 범위
    public float attackRange; // 공격 가능 범위
    public float patrolRadius; // 스폰 지점 기준 순찰 범위
    public float returnRange; // 순찰 지역 이탈 판단 범위 

    [Header("시간 설정")]
    public float moveTime; // 순찰 시간
    public float idleTime; // 대기 시간
    public float attackCooldown;

    [Header("지원군 설정")]
    public List<GameObject> reinforcementPrefabs;

    [Header("피격 반응 시스템")]
    public List<HurtReactionStep> reactionSequence; // 피격 시 체크할 행동 리스트
    public List<StateTransition> nextActionMap; // 행동 종료 후 다음 목적지 매핑

    [Header("드롭 아이템")]
    public List<DropItemData> dropTable;
}

[System.Serializable]
public struct StateTransition
{
    public string triggerKey; // "CallHelp" 등 행동 종료 시점의 키
    public HurtReactionType nextAction; // 그 다음 이어질 행동
}


[System.Serializable]
public struct DropItemData 
{
    public GameObject itemPrefab; 
    [Range(0, 1)] public float dropChance;
    public int minAmount;
    public int maxAmount;
}

public enum MonsterDisposition
{
    Passive,   
    Neutral,   
    Aggressive  
}

public enum HurtReactionType
{
    None,       
    Flee,       
    Counter,  
    CallHelp   
}

public enum AttackStyle
{
    None,       
    Basic,      
    Dash        
}

[System.Serializable]
public struct HurtReactionStep
{
    public HurtReactionType type;
    [Range(0, 100)] public float chance; // 발동 확률
    public bool stopChain; // 실행 시 다음 시퀀스를 끊을 것인가?
}

