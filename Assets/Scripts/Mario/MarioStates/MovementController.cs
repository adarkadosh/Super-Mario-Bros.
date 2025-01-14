using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 16f;
    [SerializeField] private float maxJumpHeight = 4f;
    [SerializeField] private float maxJumpTime = 1f;

    private Rigidbody2D _rigidbody;
    private PlayerInputActions _playerInputActions;

    private Vector2 _velocity;
    private float _inputX;

    public float CurrentSpeed => Mathf.Abs(_velocity.x);
    public Vector2 CurrentVelocity => _velocity;

    private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Move.performed += OnMove;
        _playerInputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Move.performed -= OnMove;
        _playerInputActions.Player.Move.canceled -= OnMove;
        _playerInputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _inputX = input.x;
    }

    private void Update()
    {
        HandleHorizontalMovement();
        ApplyGravity();
    }

    private void FixedUpdate()
    {
        ExecuteMovement();
    }

    private void HandleHorizontalMovement()
    {
        if (Mathf.Abs(_inputX) > 0.01f)
        {
            // Accelerate towards target speed
            _velocity.x = Mathf.MoveTowards(_velocity.x, _inputX * moveSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Decelerate to zero
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0f, deceleration * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        _velocity.y += Gravity * Time.deltaTime;
    }

    private void ExecuteMovement()
    {
        _rigidbody.linearVelocity = _velocity;
    }

    public void Jump()
    {
        _velocity.y = JumpForce;
    }

    // Methods to adjust movement parameters based on state
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetAcceleration(float newAcceleration)
    {
        acceleration = newAcceleration;
    }

    public void SetDeceleration(float newDeceleration)
    {
        deceleration = newDeceleration;
    }

    public void SetJumpParameters(float newJumpHeight, float newJumpTime)
    {
        maxJumpHeight = newJumpHeight;
        maxJumpTime = newJumpTime;
    }

    // Optional: Method to set velocity directly (e.g., for knockback)
    public void SetVelocity(Vector2 newVelocity)
    {
        _velocity = newVelocity;
    }

    // Optional: Method to get velocity
    public Vector2 GetVelocity()
    {
        return _velocity;
    }
}