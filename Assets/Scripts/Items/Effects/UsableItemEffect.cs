using UnityEngine;

public abstract class UsableItemEffect : ScriptableObject
{
    // 효과가 실행
    public abstract void ExecuteEffect(UsableItem parentItem, Character character);

    // 효과 설명
    public abstract string GetDescription();
}