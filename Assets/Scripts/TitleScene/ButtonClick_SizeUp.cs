using UnityEngine;
using UnityEngine.EventSystems; // 마우스 이벤트를 처리하기 위한 네임스페이스
using DG.Tweening; // DOTween 기능을 쓰기 위해 필요

public class ButtonClick_SizeUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("크기 설정")]
    public float scaleUpMultiplier = 1.1f;   // 마우스를 올렸을 때 (1.1배)
    public float scaleDownMultiplier = 0.9f; // 클릭했을 때 (0.9배)

    [Header("애니메이션 설정")]
    public float duration = 0.1f;            // 변하는 속도 (초)
    public Ease animationEase = Ease.OutQuad; // 애니메이션 부드러움 종류

    private Vector3 originalScale; // 버튼의 원래 크기를 저장할 변수

    void Awake()
    {
        // 게임 시작 시 이 버튼의 원래 크기(1, 1, 1 등)를 저장해둡니다.
        originalScale = transform.localScale;
    }

    // 마우스를 버튼 위로 올렸을 때 실행 (Hover In)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 원래 크기에서 지정한 배수만큼 키웁니다.
        transform.DOScale(originalScale * scaleUpMultiplier, duration)
                 .SetEase(animationEase)
                 .SetUpdate(true); // 일시정지 중에도 작동하게 설정
    }

    // 마우스가 버튼 영역을 벗어났을 때 실행 (Hover Out)
    public void OnPointerExit(PointerEventData eventData)
    {
        // 다시 원래 크기로 되돌립니다.
        transform.DOScale(originalScale, duration)
                 .SetEase(animationEase)
                 .SetUpdate(true);
    }

    // 마우스 왼쪽 버튼을 꾹 눌렀을 때 실행 (Press)
    public void OnPointerDown(PointerEventData eventData)
    {
        // 원래 크기보다 작게 줄여서 눌리는 느낌을 줍니다.
        transform.DOScale(originalScale * scaleDownMultiplier, duration)
                 .SetEase(animationEase)
                 .SetUpdate(true);
    }

    // 마우스 버튼에서 손을 뗐을 때 실행 (Release)
    public void OnPointerUp(PointerEventData eventData)
    {
        // 손을 뗐을 때 마우스가 아직 버튼 위에 있다면 다시 Hover 크기로, 
        // 버튼 밖에서 뗐다면 원래 크기로 돌아갑니다.
        if (eventData.pointerCurrentRaycast.gameObject == gameObject)
        {
            transform.DOScale(originalScale * scaleUpMultiplier, duration)
                     .SetEase(animationEase)
                     .SetUpdate(true);
        }
        else
        {
            transform.DOScale(originalScale, duration)
                     .SetEase(animationEase)
                     .SetUpdate(true);
        }
    }

    // 버튼이 꺼질 때 애니메이션을 강제로 멈춰서 버그를 방지합니다.
    private void OnDisable()
    {
        transform.DOKill();
        transform.localScale = originalScale;
    }
}