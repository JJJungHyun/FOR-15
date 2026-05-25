using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    // Slider 대신 렌더링이 가벼운 단일 Image 컴포넌트를 사용합니다.
    [SerializeField] private Image hpFillImage;
    [SerializeField] private bool hideOnFullHp = true;

    private Transform monsterTransform;
    private float maxHp;

    public void Init(float maxHp, Transform owner)
    {
        monsterTransform = owner;
        this.maxHp = maxHp;

        if (hpFillImage != null)
        {
            // 초기는 풀피 상태 (1.0f = 100%)
            hpFillImage.fillAmount = 1f;
        }

        if (hideOnFullHp)
        {
            gameObject.SetActive(false);
        }
    }

    public void UpdateHP(float currentHp)
    {
        if (hpFillImage == null) return;

        // 피격 시에만 활성화하여 드로우콜(Draw Call) 및 캔버스 연산 방어
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        // 단순 나눗셈 연산으로 Fill 조절 (0.0 ~ 1.0)
        hpFillImage.fillAmount = Mathf.Clamp01(currentHp / maxHp);

        // 사망 시 오브젝트 비활성화로 최적화
        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    // 몬스터 스프라이트 flipX 대응 (부모가 뒤집혀도 UI 캔버스는 월드 정방향 고정)
    private void LateUpdate()
    {
        if (monsterTransform != null)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}