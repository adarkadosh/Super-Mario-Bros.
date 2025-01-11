using UnityEngine;


public class EntityMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private Vector2 movementDirection = Vector2.left;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask sameLayerMask;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _velocity;
    
    public Vector2 MovementDirection
    {
        get => movementDirection;
        set => movementDirection = value;
    }
    public float MovementSpeed
    {
        get => movementSpeed;
        set => movementSpeed = value;
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        enabled = false;
    }

    private void OnBecameVisible()
    {
        enabled = true;
        // show me the 
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void OnEnable()
    {
    _rigidbody2D.WakeUp();
    // GetOrthographicCameraBounds();
    }

    private void OnDisable()
    {
    _rigidbody2D.linearVelocity = Vector2.zero;
    _rigidbody2D.Sleep();
    }
    
    private void FixedUpdate()
    {
        // Normalize movement direction to ensure consistent behavior
        // movementDirection = movementDirection.normalized;

        // Update horizontal velocity based on movement direction and speed
        _velocity.x = movementDirection.x * movementSpeed;

        // Apply gravity to vertical velocity
        _velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;

        // Move the Goomba
        _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * Time.fixedDeltaTime);
        Debug.DrawRay(_rigidbody2D.position, movementDirection * 0.55f, Color.red);

        // Check for obstacles ahead
        if (IsObstacleAhead())
        {
            // Reverse movement direction upon collision
            movementDirection = -movementDirection.normalized;
        }

        // Check for ground below
        if (IsGrounded())
        {
            // Prevent downward acceleration when grounded
            _velocity.y = 0f;
        }
    }

    private bool IsObstacleAhead()
    {
        Vector2 origin = _rigidbody2D.position;

        // Perform a raycast to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(origin, movementDirection, 0.55f, layerMask);

        // Check for entities on the same layer
        RaycastHit2D sameLayerHit = Physics2D.Raycast(origin, movementDirection, 0.55f);
        if (sameLayerHit.collider != null && sameLayerHit.collider.gameObject != gameObject 
                                          && sameLayerHit.collider.CompareTag("Goomba"))
        {
            Debug.Log("Same Layer Hit: " + sameLayerHit.collider.name);
            return true;
        }

        // Return true if any obstacle is detected
        return hit.collider != null;
    }

    private bool IsGrounded()
    {
        // Define the origin point slightly below the Goomba's center
        Vector2 origin = _rigidbody2D.position + Vector2.down * 0.1f;

        // Perform a raycast to detect ground directly below
        RaycastHit2D hit = Physics2D.Raycast(_rigidbody2D.position, Vector2.down, 0.55f, layerMask);
        

        // Return true if ground is detected
        return hit;
    }
}