using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;

    public PlayerAnimation playerAnimation;

    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    private Vector2 moveInput;

    private void Awake()
    {
        playerAnimation =
            new PlayerAnimation(GetComponent<Animator>());

        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 입력만 받기
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        moveInput =
            new Vector2(inputX, inputY).normalized;

        // 애니메이션 처리
        UpdateAnimation(inputX, inputY);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void UpdateAnimation(float inputX, float inputY)
    {
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
                spriteRenderer.flipX = (inputX > 0);

                playerAnimation.SetAnimState(PlayerAnimState.MoveLeft);
            }
        }
    }
}