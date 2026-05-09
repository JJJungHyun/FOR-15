using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyDatabase", menuName = "Enemy/Enemy Database")]
public class EnemyData : ScriptableObject
{
    [System.Serializable]
    public struct DropItem
    {
        public Item itemSO;
        public int minAmount;
        public int maxAmount;
        [Range(0f, 100f)] public float dropRate;
    }

    [System.Serializable]
    public class EnemySettings
    {
        [Header("Common Stats")]
        public string enemyName;
        public float hp = 20f;
        public float moveSpeed = 5f;
        public float attackDamage = 10f; // 공격력 추가
        public float attackRange = 1.5f; // 공격 사거리 추가
        public float detectRange = 8f;   // 탐지 범위
        public float chaseLimitRange = 15f; // 집(스폰지점)으로부터의 추적 제한 거리

        [Header("Drop Settings")]
        public DropItem[] drops;
    }

    public List<EnemySettings> enemyList = new List<EnemySettings>();

    public EnemySettings GetSettings(string name) => enemyList.Find(x => x.enemyName == name);
}