using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed;
    public PlayerAnimation playerAnimation;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        playerAnimation = new PlayerAnimation(GetComponent<Animator>());
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        if (inputX == 0 && inputY == 0)
        {
            playerAnimation.SetAnimState(PlayerAnimState.Idle);
        }
        else
        {
            if (inputY > 0)
            {
                playerAnimation.SetAnimState(PlayerAnimState.MoveUp);
            }
            else if (inputY < 0)
            {
                playerAnimation.SetAnimState(PlayerAnimState.MoveDown);
            }
        
            else if (inputX != 0)
            {
                // 오른쪽(>0)이면 flipX를 true, 왼쪽(<0)이면 false
                spriteRenderer.flipX = (inputX > 0);
                playerAnimation.SetAnimState(PlayerAnimState.MoveLeft);
            }
        }
    }
}
