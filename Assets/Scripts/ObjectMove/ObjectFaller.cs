using UnityEngine;
using DG.Tweening;

public class ObjectFaller : MonoBehaviour
{
    [SerializeField] private float _fallTime = 1.0f;

    public void Fall()
    {
        // 오른쪽(-90도)으로 쓰러짐, 바닥에 튕기는 효과 추가
        transform.DORotate(new Vector3(0, 0, -90f), _fallTime).SetEase(Ease.OutBounce);
    }

    // 외부에서 쓰러지는 시간을 알 수 있게 해줌 (Fader의 딜레이용)
    public float FallTime => _fallTime;
}