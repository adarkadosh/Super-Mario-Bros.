using UnityEngine;

[System.Serializable]
public class MovementData
{
    // Mirrors many of the relevant variables from MarioMoveController that the states need to access.
    // You can expand or shrink this as you see fit.

    // Velocity data
    public Vector2 velocity = Vector2.zero;
    public float inputActionsAxis = 0f;

    // Ground / Jump checks
    public bool grounded = false;
    public bool isJumping = false;
    public float jumpTimeCounter = 0f;

    // State toggles
    public bool inputEnabled = true;
    public bool isMovingToTarget = false;

    // Movement reference
    public float moveSpeed = 7f;
    public float maxJumpHeight = 4.8f;
    public float maxJumpTime = 1.15f;
    public float gravityMultiplier = 5f;
    public float onAirMultiplier = 1f;
    public float acceleration = 17f;
    public float deceleration = 25f;
    
    // Extra for autonomous movement
    public Vector3 targetPosition;
    public float autonomousSpeed;

    // Flipped state
    public bool flipped = false;
}