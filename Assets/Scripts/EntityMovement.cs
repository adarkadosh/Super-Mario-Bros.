using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class EntityMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private Vector2 movementDirection = Vector2.left;
    [SerializeField] private LayerMask layerMask;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _velocity;

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

    // private void FixedUpdate()
    // {
    //     // Move the entity in the specified direction
    //     _velocity.x = movementDirection.x * movementSpeed;
    //     // Accelerate the entity downwards
    //     _velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
    //     // Move the entity
    //     _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * Time.fixedDeltaTime);
    //     if (Physics2D.Raycast(_rigidbody2D.position, movementDirection.normalized, 0.375f, _layerMask))
    //     {
    //         Debug.DrawRay(_rigidbody2D.position, movementDirection.normalized * 0.375f, Color.red);
    //         movementDirection = -movementDirection;
    //     }
    //     if (Physics2D.Raycast(_rigidbody2D.position, Vector2.down, 0.375f, _layerMask))
    //     {
    //         _velocity.y = Mathf.Max(_velocity.y, 0f);
    //     }
    //     if (movementDirection.x > 0f) {
    //         transform.localEulerAngles = new Vector3(0f, 180f, 0f);
    //     } else if (movementDirection.x < 0f) {
    //         transform.localEulerAngles = Vector3.zero;
    //     }
    // }

    private void FixedUpdate()
    {
        // Normalize movement direction to ensure consistent behavior
        movementDirection = movementDirection.normalized;

        // Update horizontal velocity based on movement direction and speed
        _velocity.x = movementDirection.x * movementSpeed;

        // Apply gravity to vertical velocity
        _velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;

        // Move the Goomba
        _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * Time.fixedDeltaTime);

        // Check for obstacles ahead
        // if (IsObstacleAhead())
        // {
            // Visualize the raycast in the Scene view for debugging
            // Debug.DrawRay(_rigidbody2D.position, movementDirection * 0.55f, Color.red);

            // Reverse movement direction upon collision
            // movementDirection = -movementDirection;
        // }

        // Check for ground below
        if (IsGrounded())
        {
            // Prevent downward acceleration when grounded
            _velocity.y = 0f;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the Goomba collided with an obstacle
        if (collision.gameObject.layer == layerMask && collision.rigidbody != _rigidbody2D)
        {
            // Reverse movement direction upon collision
            movementDirection = -movementDirection;
        }
    }

    private bool IsObstacleAhead()
    {
        // Define the origin point slightly in front of the Goomba's center
        Vector2 origin = _rigidbody2D.position;

        // Perform a raycast to detect obstacles in the movement direction
        RaycastHit2D hit = Physics2D.Raycast(_rigidbody2D.position, movementDirection, 0.55f, layerMask);

        // Return true if an obstacle is detected
        return hit && hit.rigidbody != _rigidbody2D;
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


    void GetOrthographicCameraBounds()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera.orthographic)
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = mainCamera.orthographicSize * 2;
            float cameraWidth = cameraHeight * screenAspect;

            float left = mainCamera.transform.position.x - cameraWidth / 2;
            float right = mainCamera.transform.position.x + cameraWidth / 2;
            float top = mainCamera.transform.position.y + cameraHeight / 2;
            float bottom = mainCamera.transform.position.y - cameraHeight / 2;

            Debug.Log($"Camera Bounds:\nLeft: {left}, Right: {right}, Top: {top}, Bottom: {bottom}");
        }
        else
        {
            Debug.LogError("Camera is not orthographic!");
        }
    }
}