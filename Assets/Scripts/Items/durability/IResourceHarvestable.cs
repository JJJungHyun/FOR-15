using UnityEngine;

public interface IResourceHarvestable
{
    // 파밍 오브젝트가 타격 받았을 때 호출 (도구 종류 전송)
    void Harvest(float damage, ToolType toolType, UnityEngine.Vector2 attackerPos);
}