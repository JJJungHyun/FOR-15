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

        Vector3 moveDir = Vector3.zero;

        // 한 번에 하나의 축만 입력받도록 처리 (수평 이동에 우선순위)
        if (inputX != 0)
        {
            moveDir = new Vector3(inputX, 0, 0).normalized;
        }
        else if (inputY != 0)
        {
            moveDir = new Vector3(0, inputY, 0).normalized;
        }

        // 이동 로직
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        // 애니메이션 및 스프라이트 처리
        if (moveDir == Vector3.zero)
        {
            playerAnimation.SetAnimState(PlayerAnimState.Idle);
        }
        else
        {
            if (moveDir.y > 0)
            {
                playerAnimation.SetAnimState(PlayerAnimState.MoveUp);
            }
            else if (moveDir.y < 0)
            {
                playerAnimation.SetAnimState(PlayerAnimState.MoveDown);
            }
            else if (moveDir.x != 0)
            {
                // 오른쪽이면 flipX를 true, 왼쪽이면 false
                spriteRenderer.flipX = (moveDir.x > 0);
                playerAnimation.SetAnimState(PlayerAnimState.MoveLeft);
            }
        }
    }
}
