using UnityEngine;
using System.Collections;

public abstract class ConditionEffect
{
    public string Name { get; protected set; }
    public float Duration { get; protected set; }
    public float Power { get; protected set; }

    protected Character owner;
    protected object source;

    public ConditionEffect(Character _owner, float _duration, object _source)
    {
        owner = _owner;
        Duration = _duration;
        source = _source;
    }
    public void ResetDuration(float newDuration) => Duration = newDuration;
    public void SetPower(float newPower) => Power = newPower;
    public void Tick(float deltaTime) => Duration -= deltaTime;

    public abstract void OnStart();   // 효과 시작 
    public abstract void OnUpdate();  // 매 프레임 (도트 데미지 )
    public abstract void OnEnd();     // 효과 종료 
}