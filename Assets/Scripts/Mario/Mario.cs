using System.Collections;
using UnityEngine;

public class Mario : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Dimensions for collision checks (width & height).
    // If you have a custom collision system, adjust as necessary.
    public Vector2 dimensions = new Vector2(1f, 1f);

    // Current velocities
    private float xvel, yvel;

    // Jumping logic
    private JumpState jump;
    private bool jumping;
    private bool grounded;

    // Movement constants
    private const float conversion      = 65536;  // just used to convert fixed-point style constants
    private const float maxRunX         = 10496 / conversion;
    private const float maxWalkX        = 6400  / conversion;
    private const float walkAcc         = 152   / conversion;
    private const float runAcc          = 228   / conversion;
    private const float skidPower       = 416   / conversion;
    private const float releaseDeAcc    = 208   / conversion;

    private const float fastJumpPower   = 20480 / conversion;
    private const float jumpPower       = 16384 / conversion;
    private const float fastJumpReq     = 9472  / conversion;
    private const float modJumpReq      = 4096  / conversion;

    // Jump decays
    private const float fastJumpDecay   = 2304 / conversion;
    private const float fastJumpDecayUp = 640  / conversion;
    private const float modJumpDecay    = 1536 / conversion;
    private const float modJumpDecayUp  = 460  / conversion;
    private const float slowJumpDecay   = 1792 / conversion;
    private const float slowJumpDecayUp = 490  / conversion;

    // Air-strafe
    private const float airStrafeBorder = 6400  / conversion;
    private const float airStrafeFast   = 7424  / conversion;

    // Track whether we can fast air-strafe
    private bool fastAirStraff;

    // Cached inputs
    private bool keySpace, keyD, keyA, keyShift, keySpaceDown;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize movement
        xvel = 0f;
        yvel = 0f;
        jump = JumpState.SlowJump;
        jumping = false;
        grounded = true;
    }

    private void Update()
    {
        // Cache inputs here
        keySpaceDown = Input.GetKeyDown(KeyCode.Space);
        keySpace     = Input.GetKey(KeyCode.Space);
        keyD         = Input.GetKey(KeyCode.D);
        keyA         = Input.GetKey(KeyCode.A);
        keyShift     = Input.GetKey(KeyCode.LeftShift);
    }

    private void FixedUpdate()
    {
        // Vertical (jump) input
        if (keySpaceDown && grounded)
        {
            jumping = true;
            // Decide which jump state based on horizontal speed
            if (Mathf.Abs(xvel) > fastJumpReq)
            {
                jump = JumpState.FastJump;
                yvel = fastJumpPower;
            }
            else if (Mathf.Abs(xvel) > modJumpReq)
            {
                jump = JumpState.ModerateJump;
                yvel = jumpPower;
            }
            else
            {
                jump = JumpState.SlowJump;
                yvel = jumpPower;
            }
            // If we’re moving quickly enough in air, we can fast strafe
            fastAirStraff = Mathf.Abs(xvel) > airStrafeFast;
        }

        // Horizontal input
        bool moving   = false;
        bool skidding = false;

        // Move right
        if (keyD)
        {
            if (!grounded)
            {
                // In-air horizontal control
                if (xvel >= 0)
                {
                    xvel += (xvel >= airStrafeBorder ? runAcc : walkAcc);
                }
                else
                {
                    // Slowing mid-air
                    if (-xvel >= airStrafeBorder)
                    {
                        xvel += runAcc;
                    }
                    else
                    {
                        if (fastAirStraff) xvel += releaseDeAcc; 
                        xvel += walkAcc;
                    }
                }
            }
            else
            {
                // On ground
                moving = true;
                if (xvel >= 0)
                {
                    xvel += (keyShift ? runAcc : walkAcc);
                }
                else
                {
                    // Skidding
                    xvel += skidPower;
                    skidding = true;
                }
            }
        }

        // Move left
        if (keyA)
        {
            if (!grounded)
            {
                // In-air horizontal control
                if (xvel <= 0)
                {
                    xvel -= ( -xvel >= airStrafeBorder ? runAcc : walkAcc );
                }
                else
                {
                    // Slowing mid-air
                    if (xvel >= airStrafeBorder)
                    {
                        xvel -= runAcc;
                    }
                    else
                    {
                        if (fastAirStraff) xvel -= releaseDeAcc;
                        xvel -= walkAcc;
                    }
                }
            }
            else
            {
                // On ground
                moving = true;
                if (xvel <= 0)
                {
                    xvel -= (keyShift ? runAcc : walkAcc);
                }
                else
                {
                    // Skidding
                    xvel -= skidPower;
                    skidding = true;
                }
            }
        }

        // If not moving horizontally and on ground, apply friction
        if (!moving && grounded)
        {
            if (xvel > 0)
            {
                xvel -= releaseDeAcc;
                if (xvel < 0) xvel = 0;
            }
            else
            {
                xvel += releaseDeAcc;
                if (xvel > 0) xvel = 0;
            }
        }

        // X velocity cap (walk vs run)
        float maxSpeed = keyShift ? maxRunX : maxWalkX;
        if (xvel >  maxSpeed)  xvel =  maxSpeed;
        if (xvel < -maxSpeed)  xvel = -maxSpeed;

        // Y velocity decay (gravity-like effect)
        if (keySpace)
        {
            switch (jump)
            {
                case JumpState.FastJump:     yvel -= fastJumpDecayUp; break;
                case JumpState.ModerateJump: yvel -= modJumpDecayUp;  break;
                default:                     yvel -= slowJumpDecayUp; break;
            }
        }
        else
        {
            switch (jump)
            {
                case JumpState.FastJump:     yvel -= fastJumpDecay;   break;
                case JumpState.ModerateJump: yvel -= modJumpDecay;    break;
                default:                     yvel -= slowJumpDecay;   break;
            }
        }

        // Flip sprite depending on direction & skidding
        if (xvel > 0)
        {
            spriteRenderer.flipX = skidding;
        }
        else if (xvel < 0)
        {
            spriteRenderer.flipX = !skidding;
        }

        // We're no longer grounded until proven otherwise by collisions
        grounded = false;

        // Move according to velocity
        Vector2 moveAmount = new Vector2(xvel, yvel);
        Move(moveAmount);

        // If we jumped and are back on the ground, set jumping to false
        if (jumping) jumping = !grounded;

        // Simple example: If you fall below a certain Y, reset or handle differently
        // if (transform.position.y < -8) { /* handle fall off screen */ }

        // Update animator parameters
        animator.SetFloat("xvel", Mathf.Abs(xvel));
        animator.SetBool("skidding", skidding);
        animator.SetBool("jumping", jumping);
    }

    /// <summary>
    /// Moves Mario by 'move', then handles collisions to adjust xvel/yvel/grounded if needed.
    /// Actor.Collide is a custom collision check that you presumably have implemented.
    /// If you have a different collision system, replace with your own logic.
    /// </summary>
    private void Move(Vector2 move)
    {
        Vector2 curPos     = transform.position;
        Vector2 attemptPos = curPos + move;

        // Actor.Collide is presumably your custom function; keep or replace as needed
        CollisionInfo[] collisions = Actor.Collide(curPos, attemptPos, dimensions, 0);

        if (collisions.Length > 0)
        {
            move = HandleCollisions(move, collisions);
        }

        transform.position += new Vector3(move.x, move.y, 0);
    }

    /// <summary>
    /// Basic collision handler that stops horizontal or vertical movement
    /// depending on which side we collided on. Sets 'grounded' when hitting floor.
    /// </summary>
    private Vector2 HandleCollisions(Vector2 move, CollisionInfo[] collisions)
    {
        foreach (CollisionInfo collision in collisions)
        {
            if (collision.hitTop)
            {
                move.y = 0;
                yvel   = 0;
                grounded = true;
            }
            if (collision.hitBottom)
            {
                move.y = 0;
                yvel   = 0;
            }
            if (collision.hitRight)
            {
                move.x = 0;
                xvel   = 0;
            }
            if (collision.hitLeft)
            {
                move.x = 0;
                xvel   = 0;
            }
        }
        return move;
    }
}

internal enum JumpState
{
    SlowJump,
    ModerateJump,
    FastJump
}



/// <summary>
/// Stub for your custom collision function. Replace with your real implementation.
/// </summary>
public static class Actor
{
    public static CollisionInfo[] Collide(Vector2 startPos, Vector2 endPos, Vector2 dimensions, int someLayerMask)
    {
        // Return an empty array if no collisions.
        // Real implementation would check for collision in your tilemap or environment.
        return new CollisionInfo[0];
    }
}

/// <summary>
/// Example collision info struct from your codebase.
/// Adjust or remove if your collision system works differently.
/// </summary>
public struct CollisionInfo
{
    public bool hitTop;
    public bool hitBottom;
    public bool hitLeft;
    public bool hitRight;
    // public Actor obj; // If you have a custom 'Actor' or 'Block' class, etc.
}