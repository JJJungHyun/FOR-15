using UnityEngine;

public class DeployableObject : MonoBehaviour
{
    [Header("Lifespan Settings")]
    [SerializeField] private bool usesLifespan = true;
    [SerializeField][Tooltip("지속 시간")] private float lifespanSeconds = 5f;

    [Header("Animation Settings")]
    [Tooltip("지속시간이 몇 % 지났을 때 애니메이션을 실행할지 설정 (0.8 = 80% 지나감, 즉 20% 남았을 때)")]
    [Range(0f, 1f)]
    [SerializeField] private float animationTriggerThreshold = 0.8f;

    [Tooltip("애니메이터에 설정된 bool 파라미터 이름")]
    [SerializeField] private string endingAnimParameter = "IsEnding";

    private float timer = 0f;
    private Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기

        if (usesLifespan)
        {
            timer = lifespanSeconds;
        }
    }

    protected virtual void Update()
    {
        if (!usesLifespan) return;

        timer -= Time.deltaTime;

        if (animator != null)
        {
            float progress = (lifespanSeconds - timer) / lifespanSeconds;

            if (progress >= animationTriggerThreshold)
            {
                animator.SetBool(endingAnimParameter, true);
            }
            else
            {
                animator.SetBool(endingAnimParameter, false);
            }
        }

        if (timer <= 0f)
        {
            OnLifespanExpired();
        }
    }

    protected virtual void OnLifespanExpired()
    {
        Debug.Log($"{gameObject.name}의 지속 시간이 다되어 소멸합니다.");
        Destroy(gameObject);
    }
}