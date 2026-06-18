using UnityEngine;
using TMPro; // TextMeshPro 지정을 위해 필수
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class EscapeTimer : MonoBehaviour
{
    [Header("Timer Setting")]
    [SerializeField] private float timeRemaining = 60f; // 60초 제한시간
    [SerializeField] private TextMeshProUGUI timerText; // 화면에 째깍거릴 텍스트 UI 연결창

    [Header("Scene Setting")]
    [Tooltip("60초가 끝났을 때 이동할 타이틀 씬의 정확한 이름")]
    [SerializeField] private string titleSceneName = "TitleScene";

    private bool isTimerRunning = false;

    private void Start()
    {
        // 마지막 씬이 열리자마자 바로 타이머 가동 시작!
        isTimerRunning = true;
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // 매 프레임 시간 깎기
            UpdateTimerDisplay(timeRemaining);
        }
        else
        {
            // 0초가 되는 순간!
            timeRemaining = 0;
            isTimerRunning = false;
            GoToTitle(); // 타이틀로 보내버리기
        }
    }

    // "00:60", "00:59" 형태로 UI 텍스트 업데이트
    private void UpdateTimerDisplay(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // 💡 10초 남으면 텍스트를 빨간색으로 바꿔서 긴박감 주기
            if (timeToDisplay <= 10f)
            {
                timerText.color = Color.red;
            }
        }
    }

    // ★ 시간 초과 시 타이틀 씬으로 이동시키는 핵심 함수
    private void GoToTitle()
    {
        Debug.Log("60초 시간 초과! 타이틀 씬으로 즉시 강제 이동합니다.");
        SceneManager.LoadScene(titleSceneName);
    }

    // (보너스) 제한 시간 내에 탈출 성공 시 타이머를 멈춰 세우기 위한 함수
    public void StopTimer()
    {
        isTimerRunning = false;
    }
}