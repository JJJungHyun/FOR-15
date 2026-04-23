using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    public GuideArrow guideArrow;

    [SerializeField] private string _nextSceneName = "TitleScene";
    private bool _isCleared = false;

    private void OnTriggerEnter2D(Collider2D foreign)
    {
        if (foreign.CompareTag("Player"))
        {
            guideArrow.SetInsideZone(true);

            if (!_isCleared)
            {
                _isCleared = true;
                HandleGameClear();
            }
        }
    }

    private void HandleGameClear()
    {
        Debug.Log("탈출 성공! 게임을 종료하거나 다음 씬으로 이동합니다.");

        // 방법 1: MySceneManager를 이용해 결과 씬으로 이동
        if (MySceneManager.Instance != null)
        {
            MySceneManager.Instance.ChangeScene(_nextSceneName);
        }
        else
        {
            // 매니저가 없을 경우를 대비한 일반 로드 방식 (비상용)
            UnityEngine.SceneManagement.SceneManager.LoadScene(_nextSceneName);
        }
    }

    private void OnTriggerExit2D(Collider2D foreign)
    {
        if (foreign.CompareTag("Player"))
        {
            guideArrow.SetInsideZone(false);
        }
    }
}