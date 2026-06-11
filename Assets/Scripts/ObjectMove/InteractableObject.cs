using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isDisappearing = false;
    private bool isPlayerNearby = false; // 플레이어가 상호작용 범위 내에 있는지 확인

    [SerializeField] private float fadeDuration = 1.0f; // 사라지는 데 걸리는 시간 (초)

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 플레이어가 근처에 있고, F 키를 눌렀고, 이미 사라지는 중이 아닐 때
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F) && !isDisappearing)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    IEnumerator FadeOutAndDestroy()
    {
        isDisappearing = true;
        Color originalColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // 시간에 따라 알파 값을 1에서 0으로 보간(Lerp)
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

            yield return null; // 다음 프레임까지 대기
        }

        // 완전히 사라지면 오브젝트 파괴 (또는 비활성화)
        Destroy(gameObject);
    }

    // 플레이어 감지용 콜라이더 체크 (Trigger 세팅 필요)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerNearby = false;
    }
}