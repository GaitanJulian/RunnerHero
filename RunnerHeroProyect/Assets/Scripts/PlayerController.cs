using UnityEngine;
/// <summary>
/// Controls the movement of the player character.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterStatsScriptableObject characterStats;

    private bool isJumping;
    private float jumpCounter; // Handles how much extra time the player can jump to get extra height
    private bool jumpBuffer;
    private Vector2 gravity;
    private Rigidbody2D rb;
    public Transform groundCheck; // helps handling the isGrounded() method with an horizontal capsule draw at this positon
    private Vector2 capsuleSize = new Vector2(0.31f, 0.06f); // Size obtained by visually measuring the capsule in the scene at the specified Trannsform
    public LayerMask groundMask; // same as groundCheck

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

        // Handles a jump buffer for better UX
        if (Input.GetButtonDown("Jump") && !isGrounded())
            jumpBuffer = true;

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpCounter = 0;
            jumpBuffer = false;

            // If the player Stops jumping then the speeds decay by the percentage specified

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * characterStats.jumpDecayPercentage);
            }
        }

        moveInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        // Handles dynamic jumping and dynamic fall
        if (rb.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.fixedDeltaTime;
            if (jumpCounter > characterStats.jumpTime)
                isJumping = false;

            float t = jumpCounter / characterStats.jumpTime; // Normalized jump calcule
            float currentJump = characterStats.jumpMultiplier;

            // If the jump is more than halfway through, then apply this formula to generate a more realistic jump
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

        // Apply horizontal movement, if the player is not pressing any move key then the character stops
        rb.velocity = new Vector2(moveInput * characterStats.moveSpeed, rb.velocity.y);

    }

    /// <summary>
    /// Checks if the player is grounded.
    /// </summary>
    /// <returns><c>true</c> if the player is grounded; otherwise, <c>false</c>.</returns>
    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, capsuleSize, CapsuleDirection2D.Horizontal, 0, groundMask);
    }
}
