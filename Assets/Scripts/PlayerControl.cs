using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Start()
    {

    }
    void Update()
    {
        // 방향키 입력
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // 입력 방향 정규화
        Vector3 normalizedInput = new Vector3(inputX, inputY, 0).normalized;

        // 이동 로직
        transform.Translate(normalizedInput * moveSpeed * Time.deltaTime);
    }
}
