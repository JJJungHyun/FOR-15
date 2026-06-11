using UnityEngine;

public class Quit : MonoBehaviour
{
    // 유니티 에디터에서 팝업창 UI 오브젝트(Panel 등)를 여기에 드래그 앤 드롭합니다.
    [SerializeField] private GameObject quitPopupPanel;

    void Start()
    {
        // 게임 시작할 때는 팝업창이 보이지 않도록 숨깁니다.
        if (quitPopupPanel != null)
        {
            quitPopupPanel.SetActive(false);
        }
    }

    // 1. 팝업창 켜기
    public void ShowQuitPopup()
    {
        if (quitPopupPanel != null)
        {
            quitPopupPanel.SetActive(true);
        }
    }

    // 2. 취소 버튼
    public void CancelQuit()
    {
        if (quitPopupPanel != null)
        {
            quitPopupPanel.SetActive(false);
        }
    }

    // 3.  게임 종료
    public void ConfirmQuit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}