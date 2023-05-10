using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStatsScriptableObject characterStats;

    private bool isJumping;
    private float jumpCounter;
    private bool jumpBuffer;
    private float jumpTimeCounter;
    private Vector2 gravity;

    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundMask;

    private bool isMoving;
    private float moveInput;
    private float moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        isJumping = false;
        gravity = new Vector2(0, -Physics2D.gravity.y);
    }

    void Update()
    {
        if ((Input.GetButtonDown("Jump") || jumpBuffer) && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, characterStats.jumpForce);
            isJumping = true;
            jumpCounter = 0;
            jumpBuffer = false;
        }

        if (Input.GetButtonDown("Jump") && !isGrounded())
            jumpBuffer = true;

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpCounter = 0;

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        isMoving = (moveInput != 0f);
    }

    void FixedUpdate()
    {
        if (rb.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.fixedDeltaTime;
            if (jumpCounter > characterStats.jumpTime)
                isJumping = false;

            float t = jumpCounter / characterStats.jumpTime;
            float currentJump = characterStats.jumpMultiplier;

            if (t > 0.5f)
            {
                currentJump = characterStats.jumpMultiplier * (1 - t);
            }

            rb.velocity += gravity * currentJump * Time.fixedDeltaTime;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity -= gravity * characterStats.fallMultiplier * Time.fixedDeltaTime;
        }

        moveDirection = moveInput * characterStats.moveSpeed;

        // Apply acceleration or deceleration
        float acceleration = isMoving ? characterStats.acceleration : characterStats.deceleration;
        float deceleration = isMoving ? characterStats.deceleration : characterStats.acceleration;
        float targetVelocityX = moveDirection;

        if (targetVelocityX != 0f)
        {
            float currentVelocityX = rb.velocity.x;
            float newVelocityX = Mathf.MoveTowards(currentVelocityX, targetVelocityX, acceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
        }
        else
        {
            float currentVelocityX = rb.velocity.x;
            float newVelocityX = Mathf.MoveTowards(currentVelocityX, 0f, deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newVelocityX, rb.velocity.y);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.31f, 0.06f), CapsuleDirection2D.Horizontal, 0, groundMask);
    }
}