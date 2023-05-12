using UnityEngine;
/// <summary>
/// Controls the movement of the player character.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStatsScriptableObject characterStats;

    private bool isJumping;
    private float jumpCounter;
    private bool jumpBuffer;
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
        // Handles jumping logic
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
        // Handles continuous jumping and falling
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
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            rb.AddForce(new Vector2(characterStats.moveSpeed * moveInput, 0), ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Checks if the player is grounded.
    /// </summary>
    /// <returns><c>true</c> if the player is grounded; otherwise, <c>false</c>.</returns>
    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.31f, 0.06f), CapsuleDirection2D.Horizontal, 0, groundMask);
    }
}
