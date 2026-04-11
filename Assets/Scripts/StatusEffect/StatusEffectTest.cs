using UnityEngine;

public class StatusEffectTest : MonoBehaviour
{
    [Header("연결 설정")]
    [SerializeField] private Character playerCharacter;
    [SerializeField] private CharConditionHandler conditionHandler;

    [Header("독 파라미터")]
    [SerializeField] private float poisonDuration = 5f;
    [SerializeField] private float poisonDamage = 2f;

    public void AddPoison()
    {
        if (playerCharacter != null && conditionHandler != null)
        {
            PoisonEffect newPoison = new PoisonEffect(playerCharacter, poisonDuration, poisonDamage, this);

            conditionHandler.ApplyEffect(newPoison);

            Debug.Log($"<color=purple>[테스트]</color> 독 적용! {poisonDuration}초 동안 초당 {poisonDamage} 데미지");
        }
        else
        {
            Debug.LogError("PlayerCharacter 또는 ConditionHandler가 할당되지 않았습니다!");
        }
    }
}