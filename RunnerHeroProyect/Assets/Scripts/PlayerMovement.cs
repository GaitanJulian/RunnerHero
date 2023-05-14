using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterStatsScriptableObject characterStats;
    private PlayerInputActions playerControls;
    private InputAction move;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        move = playerControls.Player.Move;
    }

    private void OnEnable()
    {
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
