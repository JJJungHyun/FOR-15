using UnityEngine;
using DG.Tweening;

public class ObjectFader : MonoBehaviour
{
    [SerializeField] private float _fadeTime = 1.0f;
    private SpriteRenderer _sprite;

    [SerializeField] private GameObject _itemPrefab; // 드롭될 아이템 프리팹
    [SerializeField] private int _dropCount = 1;      // 생성할 아이템 개수

    void Awake()
    {
        // GetComponent 대신 GetComponentInChildren을 사용합니다.
        _sprite = GetComponentInChildren<SpriteRenderer>();

        // 혹시 못 찾을 경우를 대비해 확인용 로그 추가
        if (_sprite == null)
        {
            Debug.LogError("자식 오브젝트에서 SpriteRenderer를 찾을 수 없습니다!");
        }
    }

    public void FadeOut(float delay)
    {
        // 쓰러지는 시간만큼 기다렸다가(delay) 투명해짐
        _sprite.DOFade(0f, _fadeTime).SetDelay(delay).OnComplete(() => {
            DropItem();
            Destroy(gameObject); // 완전히 사라지면 오브젝트 삭제
        });
    }
    private void DropItem()
    {
        if (_itemPrefab != null)
        {
            for (int i = 0; i < _dropCount; i++)
            {
                // 나무의 현재 위치에 아이템 생성
                Instantiate(_itemPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}