using UnityEngine;

public interface IDetectionCondition
{
    bool IsSatisfied(MonsterController owner, Transform player);
}