using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Controls the movement of the player character.
/// </summary>
public class PlayerJump : MonoBehaviour
{
    [SerializeField] private CharacterStatsScriptableObject characterStats;

    private PlayerInputActions playerControls;
    private InputAction jump;

    private bool isJumping;
    private float jumpCounter; // Handles how much extra time the player can jump to get extra height
    private bool jumpBuffer; // to check if the player is using the buffer

    private Vector2 gravity; // to operate easier with gravity

    private Rigidbody2D rb;
    public Transform groundCheck; // helps handling the isGrounded() method with an horizontal capsule draw at this positon
    private Vector2 capsuleSize = new Vector2(0.31f, 0.06f); // Size obtained by visually measuring the capsule in the scene at the specified Transform
    public LayerMask groundMask; // Layer of the ground to detect whenever the player touch the ground

    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerControls = new PlayerInputActions();
        jump = playerControls.Player.Jump;
    }

    private void OnEnable()
    {
        jump.Enable();
        jump.performed += OnJumpPerformed;
        jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        jump.Disable();
        jump.performed -= OnJumpPerformed;
        jump.canceled -= OnJumpCanceled;
    }

    void Start()
    {
        isJumping = false;
        gravity = new Vector2(0, -Physics2D.gravity.y);
    }

    void Update()
    {
        // Handles a jump buffer for better UX
        if (jump.WasPerformedThisFrame() && !isGrounded())
            jumpBuffer = true;
    }

    void FixedUpdate()
    {
        // Handles jump logic
        if ((isJumping || jumpBuffer) && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, characterStats.jumpForce);
            jumpCounter = 0;
            jumpBuffer = false;
        }

        if (!isJumping)
        {
            jumpCounter = 0;
            jumpBuffer = false;

            // If the player stops jumping, decay the vertical speed by the specified percentage
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * characterStats.jumpDecayPercentage);
            }
        }

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
        // rb.velocity = new Vector2(moveInput * characterStats.moveSpeed, rb.velocity.y);

    }

    /// <summary>
    /// Checks if the player is grounded.
    /// </summary>
    /// <returns><c>true</c> if the player is grounded; otherwise, <c>false</c>.</returns>
    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, capsuleSize, CapsuleDirection2D.Horizontal, 0, groundMask);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        isJumping = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumping = false;
    }

}
