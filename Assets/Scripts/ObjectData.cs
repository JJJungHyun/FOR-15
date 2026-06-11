using UnityEngine;
using System.Collections.Generic;

public enum ObjectType { Tree, Rock }
public enum ObjectSize { Small, Medium, Large }

[System.Serializable]
public struct ObjDropItem
{
    public Item itemData;                    // 드롭될 아이템의 SO 데이터
    [Range(0f, 1f)] public float dropChance; // 0.0 ~ 1.0 확률
    public int minAmount;                    // 최소 개수
    public int maxAmount;                    // 최대 개수
}

[System.Serializable]
public struct SizeDependentDrop
{
    public ObjectSize size;
    [Range(0.5f, 2.0f)] public float minScale;
    [Range(0.5f, 2.0f)] public float maxScale;
    public List<ObjDropItem> dropTable;
}

[CreateAssetMenu(fileName = "NewObjectData", menuName = "ScriptableObjects/ObjectData")]
public class ObjectData : ScriptableObject
{
    public ObjectType objectType;

    [Header("드롭 시스템")]
    [Tooltip("아이템 드롭에 공용으로 사용할 기본 베이스 프리팹")]
    public GameObject defaultItemPrefab;     

    [Header("크기 및 크기별 드롭 설정")]
    public List<SizeDependentDrop> sizeSettings;
}