using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    public GuideArrow guideArrow;

    [SerializeField] private string _nextSceneName = "TitleScene";
    [SerializeField] private string _cutsceneName = "GameClearCutscene";
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

        if (CutSceneManager.Instance != null)
        {
            CutSceneManager.Instance.StartCutscene(_cutsceneName);
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