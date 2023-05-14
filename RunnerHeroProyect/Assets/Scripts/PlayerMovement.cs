using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterStatsScriptableObject characterStats;

    private PlayerInputActions playerControls;
    private InputAction move;

    private Rigidbody2D rb;

    private Vector2 playerInput;
    private Vector2 desiredSpeed;
    private Vector2 currentSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }

    private void Update()
    {
        playerInput = move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (playerInput != Vector2.zero)
        {
            desiredSpeed = new Vector2( playerInput.x * characterStats.maxSpeed, rb.velocity.y);
            currentSpeed = rb.velocity;
            rb.velocity = Vector2.Lerp(currentSpeed, desiredSpeed, characterStats.acceleration * Time.fixedDeltaTime);
        }
        else
        {
            desiredSpeed = new Vector2(0, rb.velocity.y);
            currentSpeed = rb.velocity;
            rb.velocity = Vector2.Lerp(currentSpeed, desiredSpeed, characterStats.deceleration * Time.fixedDeltaTime);
        }
    }
}
