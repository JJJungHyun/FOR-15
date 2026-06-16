using UnityEngine;

public class KeepOnLoad : MonoBehaviour
{
    private void Awake()
    {
        // 이 오브젝트는 씬이 바뀌어도 파괴되지 않고 유지됩니다.
        DontDestroyOnLoad(gameObject);
    }
}