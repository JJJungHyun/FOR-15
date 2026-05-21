using UnityEngine;

public class DeployableObject : MonoBehaviour
{
    [Header("Lifespan Settings")]
    [SerializeField] private bool usesLifespan = true;
    [SerializeField]
    [Tooltip("지속 시간 (초 단위, 기획상 하루 5분 = 300초)")]
    private float lifespanSeconds = 300f;

    private float timer = 0f;

    protected virtual void Start()
    {
        // 생성 시점에 충돌 연산을 위해 트리거 상태나 레이어를 세팅할 수 있습니다.
        if (usesLifespan)
        {
            timer = lifespanSeconds;
        }
    }

    protected virtual void Update()
    {
        if (!usesLifespan) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            OnLifespanExpired();
        }
    }

    protected virtual void OnLifespanExpired()
    {
        // 이펙트를 뿌리거나 사운드를 내고 삭제
        Debug.Log($"{gameObject.name}의 지속 시간이 다되어 소멸합니다.");
        Destroy(gameObject);
    }
}