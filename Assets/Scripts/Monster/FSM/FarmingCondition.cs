using UnityEngine;

public class FarmingCondition : IDetectionCondition
{
    public bool IsSatisfied(MonsterController owner, Transform player)
    {
        var playerCtrl = player.GetComponent<Character>();
        return playerCtrl != null && playerCtrl.IsFarming;
    }
}