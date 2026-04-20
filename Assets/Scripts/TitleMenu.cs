using UnityEngine;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    public Button continueButton;
    public string gameSceneName = "MainScene"; // 실제 이동할 씬 이름

    private void Start()
    {
        // 세이브 데이터가 있는지 확인해서 이어하기 버튼 활성화/비활성화
        if (GameSaveManager.Instance != null && continueButton != null)
        {
            continueButton.interactable = GameSaveManager.Instance.HasSaveData();
        }
    }

    public void StartNewGame()
    {
        // 1. 이어하기 플래그 꺼주기
        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.isContinueGame = false;

        // 2. [중요] MySceneManager의 ChangeScene 호출 (DOTween 애니메이션 시작)
        if (MySceneManager.Instance != null)
        {
            MySceneManager.Instance.ChangeScene(gameSceneName);
        }
        else
        {
            Debug.LogError("MySceneManager가 씬에 없습니다!");
        }
    }

    public void ContinueGame()
    {
        // 1. 이어하기 플래그 켜주기
        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.isContinueGame = true;

        // 2. [중요] MySceneManager의 ChangeScene 호출
        if (MySceneManager.Instance != null)
        {
            MySceneManager.Instance.ChangeScene(gameSceneName);
        }
        else
        {
            Debug.LogError("MySceneManager가 씬에 없습니다!");
        }
    }
}