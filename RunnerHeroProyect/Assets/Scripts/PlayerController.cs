using UnityEngine;


public class PlayerController : MonoBehaviour
{

    // Player movement stats

    [SerializeField] private CharacterStatsScriptableObject characterStats;

    private bool isJumping;
    private float jumpCounter;

    private float jumpTimeCounter;
    private Vector2 gravity;

    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {  
        isJumping = false;
        gravity = new Vector2(0, -Physics2D.gravity.y);
    }

    void Update()
    {
        // Player Jump
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, characterStats.jumpForce);
            isJumping = true;
            jumpCounter = 0;
        }

        // Player stopped jumping
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpCounter = 0;

            // If the player stoped jumping but the speed is still positive, then in every update frame the speed will decay 40%
            if( rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }

        }
    }

    void FixedUpdate()
    {
        // Jump multiplier that depends on the player pressing jump button
        if (rb.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.fixedDeltaTime;
            if (jumpCounter > characterStats.jumpTime) isJumping = false;

            float t = jumpCounter / characterStats.jumpTime;
            float currentJump = characterStats.jumpMultiplier;

            if (t > 0.5f)
            {
                currentJump = characterStats.jumpMultiplier * (1 - t);
            }

            rb.velocity += gravity * currentJump * Time.fixedDeltaTime;
        }

        // Smoother fall of the player
        if (rb.velocity.y < 0)
        {
            rb.velocity -= gravity * characterStats.fallMultiplier * Time.fixedDeltaTime;
        }
    }
    private bool isGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.31f, 0.06f), CapsuleDirection2D.Horizontal, 0, groundMask);
    }
}

