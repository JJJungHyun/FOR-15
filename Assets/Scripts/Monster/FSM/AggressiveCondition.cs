using UnityEngine;

public class AggressiveCondition : IDetectionCondition
{
    public bool IsSatisfied(MonsterController owner, Transform player) => true;
}