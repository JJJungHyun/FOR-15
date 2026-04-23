using UnityEngine;
using DG.Tweening; // DOTween 필수

public class ItemPopUp : MonoBehaviour
{
    [Header("점프 설정")]
    [SerializeField] private float _jumpPower = 2.0f;     // 튀어 오르는 높이
    [SerializeField] private float _jumpDuration = 0.5f;  // 떨어지는 시간
    [SerializeField] private float _dropRange = 1.5f;     // 옆으로 퍼지는 범위

    public void PlayDropAnimation()
    {
        // 1. 초기화: 나타날 때 크기를 0에서 시작
        transform.localScale = Vector3.zero;

        // 2. 떨어질 지점 계산 (현재 위치에서 좌우 랜덤하게)
        float randomX = Random.Range(-_dropRange, _dropRange);
        Vector3 targetPos = transform.position + new Vector3(randomX, -0.5f, 0);

        // 3. 위로 튀어 올랐다 떨어지는 애니메이션 (DOJump)
        // (목표위치, 점프높이, 점프횟수, 시간)
        transform.DOJump(targetPos, _jumpPower, 1, _jumpDuration).SetEase(Ease.OutQuad);

        // 4. 크기가 커지는 연출 (보너스)
        transform.DOScale(Vector3.one, _jumpDuration).SetEase(Ease.OutBack);

        // 5. 회전 연출 (보너스: 살짝 돌면서 떨어짐)
        transform.DORotate(new Vector3(0, 0, Random.Range(-90f, 90f)), _jumpDuration);
    }
}