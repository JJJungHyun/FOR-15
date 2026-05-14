using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "ScriptableObjects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("몬스터 성향")]
    public MonsterDisposition disposition;
    public HurtReactionType hurtReaction;
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

    [Range(0f, 1f)]
    public float callHelpChance;

    [Header("드롭 아이템")]
    public List<DropItemData> dropTable;
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