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

    private float moveInput;

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
            jumpBuffer = false;
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }
        }

        moveInput = Input.GetAxisRaw("Horizontal");
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

        // Apply horizontal movement
        float targetVelocityX = moveInput * characterStats.moveSpeed;
        float currentVelocityX = rb.velocity.x;
        float acceleration = isGrounded() ? characterStats.acceleration : characterStats.airAcceleration;
        float deceleration = isGrounded() ? characterStats.deceleration : characterStats.airDeceleration;

        // Apply acceleration or deceleration
        if (Mathf.Abs(targetVelocityX) > 0.1f)
        {
            float velocityDiffX = targetVelocityX - currentVelocityX;
            float accelX = Mathf.Clamp(velocityDiffX * acceleration, -acceleration, acceleration);
            rb.AddForce(new Vector2(accelX, 0f));
        }
        else
        {
            float decelX = Mathf.Sign(currentVelocityX) * deceleration;
            rb.AddForce(new Vector2(decelX, 0f));
        }

    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.31f, 0.06f), CapsuleDirection2D.Horizontal, 0, groundMask);
    }

    
}