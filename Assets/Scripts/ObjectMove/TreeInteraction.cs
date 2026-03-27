using UnityEngine;

public class TreeInteraction : MonoBehaviour
{
    private ObjectFaller _faller;
    private ObjectFader _fader;
    private bool _isPlayerInRange = false; // 플레이어가 범위 안에 있는지 확인
    private bool _isActionDone = false;    // 이미 쓰러졌는지 확인

    void Start()
    {
        _faller = GetComponent<ObjectFaller>();
        _fader = GetComponent<ObjectFader>();
    }

    void Update()
    {
        // 범위 안에 있고, F키를 눌렀고, 아직 쓰러지지 않았다면 실행
        if (_isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !_isActionDone)
        {
            DoAction();
        }
    }

    private void DoAction()
    {
        Debug.Log("나무가 쓰러집니다.");
        _isActionDone = true; // 중복 실행 방지

        _faller.Fall(); // 쓰러지기 시작
        _fader.FadeOut(_faller.FallTime); // 쓰러지는 시간만큼 기다린 후 사라지기 시작
    }

    // 플레이어가 감지 범위에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInRange = true;
            Debug.Log("F키를 눌러 나무와 상호작용 하세요.");
        }
    }

    // 플레이어가 감지 범위를 벗어났을 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPlayerInRange = false;
        }
    }
}