using UnityEngine;

public class TreeInteraction : MonoBehaviour
{
    [Header("거리 설정")]
    [SerializeField] private float _interactRange = 3.0f; // 색상 변경 스크립트와 똑같이 맞추세요

    private ObjectFaller _faller;
    private ObjectFader _fader;
    private Transform _playerTransform;
    private bool _isActionDone = false;

    void Start()
    {
        _faller = GetComponent<ObjectFaller>();
        _fader = GetComponent<ObjectFader>();

        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    void Update()
    {
        if (_isActionDone || _playerTransform == null) return;

        // 1. 플레이어와의 거리 계산 (색상 스크립트와 동일한 로직)
        float distance = Vector2.Distance(transform.position, _playerTransform.position);

        // 2. 거리 안에 있을 때만(파란색 상태일 때만) F키 입력 허용
        if (distance <= _interactRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                DoAction();
            }
        }
    }

    private void DoAction()
    {
        Debug.Log("나무가 파란색 상태이므로 쓰러집니다.");
        _isActionDone = true;

        _faller.Fall();
        _fader.FadeOut(_faller.FallTime);
    }
}