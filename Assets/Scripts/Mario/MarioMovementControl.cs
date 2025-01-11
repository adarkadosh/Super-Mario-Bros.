using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static Extensions;

[RequireComponent(typeof(Rigidbody2D))]
public class MarioMovementControl : MonoBehaviour
{
    private Camera _mainCamera;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private Rigidbody2D _rigidbody;
    private Collider2D _capsuleCollider;
    [SerializeField] private float offsetTime = 1f;

    [SerializeField] private Vector2 velocity;
    private float _inputActionsAxis;

    [SerializeField] private Vector3 offset = new Vector3(2.5f, 0, 0); // Offset when moving

    [SerializeField]
    private float centerThreshold = 0.14f; // Range near the center where no offset is applied

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private float maxJumpTime = 1f;
    private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    private bool Grounded { get; set; }
    public bool Jumping { get; private set; }
    public bool Running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(_inputActionsAxis) > 0.25f;

    public bool Sliding => (_inputActionsAxis > 0f && velocity.x < 0f) ||
                           (_inputActionsAxis < 0f && velocity.x > 0f);

    public bool Falling => velocity.y < 0f && !Grounded;

    private PlayerInputActions _playerInputActions;
    [SerializeField] private LayerMask layerMask;
    private CinemachinePositionComposer _cinemachinePositionComposer;


    private void Start()
    {
        _cinemachinePositionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<Collider2D>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
    }

    private void OnEnable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _capsuleCollider.enabled = true;
        velocity = Vector2.zero;
        Jumping = false;

        _playerInputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _capsuleCollider.enabled = false;
        velocity = Vector2.zero;
        _inputActionsAxis = 0f;
        Jumping = false;

        _playerInputActions.Player.Jump.performed -= OnJump;
    }

    private void Update()
    {
        HorizontalMovement();

        Grounded = Physics2D.CircleCast(
            _rigidbody.position + new Vector2(0, 0.5f), 0.25f, Vector2.down, 0.375f, layerMask);
        if (Grounded)
        {
            GroundedMovement();
        }

        ApplyGravity();
        DrawCircleCast(_rigidbody.position + new Vector2(0, 0.5f), 0.25f, Vector2.down, 0.375f);

        // TODO: fix camera offset when running (cinemachine)
        // if ((_velocity.x) > 2f)
        // {
        //     _cinemachinePositionComposer.TargetOffset = new Vector3(0, 0, 0);
        // }
        // else
        // {
        //     _cinemachinePositionComposer.TargetOffset = new Vector3(2.5f, 0, 0);
        // }
        // Get Mario's position in viewport coordinates
        // Vector3 screenPosition = _mainCamera.WorldToViewportPoint(transform.position);
        // // Debug.Log(screenPosition);
        //
        // // Check if Mario is in the middle of the screen
        // bool isCentered = screenPosition.x <= 0.5f && screenPosition.x >= 0.5f - centerThreshold;
        //
        // if (isCentered && !Running)
        // {
        //     _cinemachinePositionComposer.TargetOffset = Vector3.zero;
        //     // Set the camera offset to zero when Mario is centered
        // }
        // else if (_rigidbody.linearVelocity.x > 0.1f)
        // {
        //     // Apply the offset based on direction
        //     // _cinemachinePositionComposer.TargetOffset = Vector3.Lerp(
        //         // _cinemachinePositionComposer.TargetOffset, offset, Time.deltaTime * offsetTime);
        //         _cinemachinePositionComposer.TargetOffset = Vector3.Lerp(
        //             _cinemachinePositionComposer.TargetOffset, Vector3.zero,
        //             Time.deltaTime * offsetTime // Adjust transition speed as needed
        //         );
        // }
        // else
        // {
        //     if (velocity.x > 0) _cinemachinePositionComposer.TargetOffset = offset;
        // }
    }

    private void FixedUpdate()
    {
        // Move mario based on his velocity
        var position = _rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        // Clamp within the screen bounds
        Vector2 leftEdge = _mainCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        _rigidbody.MovePosition(position);
    }

    private void HorizontalMovement()
    {
        // Accelerate / decelerate
        _inputActionsAxis = _playerInputActions.Player.Move.ReadValue<Vector2>().x;
        velocity.x =
            Mathf.MoveTowards(velocity.x, _inputActionsAxis * moveSpeed, moveSpeed * Time.deltaTime);

        DrawCircleCast(_rigidbody.position + new Vector2(0, 0.5f), 0.25f, Vector2.right * velocity.x,
            0.375f);
        // Check if running into a wall
        if (Physics2D.CircleCast(_rigidbody.position + new Vector2(0, 0.5f), 0.25f,
                Vector2.right * velocity.x, 0.375f, layerMask))
        {
            velocity.x = 0f;
        }

        transform.eulerAngles = velocity.x switch
        {
            // Flip sprite to face direction
            > 0f => Vector3.zero,
            < 0f => new Vector3(0f, 180f, 0f),
            _ => transform.eulerAngles
        };
    }

    private void GroundedMovement()
    {
        // Prevent gravity from infinity building up
        velocity.y = Mathf.Max(velocity.y, 0f);
        Jumping = velocity.y > 0f;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jumping");
        if (Grounded)
        {
            velocity.y = JumpForce;
            Jumping = true;
        }
    }

    private void ApplyGravity()
    {
        // Check if falling
        var falling = velocity.y < 0f || !_playerInputActions.Player.Jump.IsPressed();
        var multiplier = falling ? 2f : 1f;

        // Apply gravity and terminal velocity
        velocity.y += Gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, Gravity / 2f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 direction = other.transform.position - transform.position;
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Bounce off enemy head
            if (Vector2.Dot(direction.normalized, Vector2.down) > 0.25f)
            {
                velocity.y = JumpForce / 2f;
                Jumping = true;
            }
        }
        else if (other.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // Stop vertical movement if mario bonks his head
            if (Vector2.Dot(direction.normalized, Vector2.up) > 0.25f)
            {
                velocity.y = 0f;
            }
        }
    }
}