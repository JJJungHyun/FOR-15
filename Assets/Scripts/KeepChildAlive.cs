using UnityEngine;

public class KeepChildAlive : MonoBehaviour
{
    private static KeepChildAlive instance;

    void Awake()
    {
        // ★ 핵심: 부모 오브젝트가 있다면 관계를 끊어서 독립된 최상위 오브젝트로 만듭니다.
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        // 싱글톤 처리: 씬이 바뀔 때 중복으로 생성되는 것을 방지합니다.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 이제 부모가 없으므로 안전하게 파괴를 막을 수 있습니다!
        }
        else
        {
            // 이미 이전 씬에서 살아남아 넘어온 동일한 오브젝트가 있다면, 새로 생성된 것은 삭제
            Destroy(gameObject);
        }
    }
}