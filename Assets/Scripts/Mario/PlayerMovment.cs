using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Horizontal Movement")] public float moveSpeed = 10f;
    public Vector2 direction;
    private bool _facingRight = true;

    [Header("Vertical Movement")] public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float _jumpTimer;

    [Header("Components")] public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;
    public GameObject characterHolder;

    [Header("Physics")] public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")] public bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 colliderOffset;

    // Update is called once per frame
    void Update()
    {
        // bool wasOnGround = onGround;
        onGround =
            Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
            Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);


        if (Input.GetButtonDown("Jump"))
        {
            _jumpTimer = Time.time + jumpDelay;
        }

        // animator.SetBool("onGround", onGround);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        DOMoveCharacter(direction.x);
        if (_jumpTimer > Time.time && onGround)
        {
            Jump();
        }

        DOModifyPhysics();
    }

    private void DOMoveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * (horizontal * moveSpeed));

        if ((horizontal > 0 && !_facingRight) || (horizontal < 0 && _facingRight))
        {
            Flip();
        }

        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
        }

        // animator.SetFloat("horizontal", Mathf.Abs(rb.linearVelocity.x));
        // animator.SetFloat("vertical",rb.linearVelocity.y);
        animator.SetBool("Walking", horizontal != 0);
        animator.SetBool("IsJumping", !rb.linearVelocity.y.Equals(0));
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        _jumpTimer = 0;
    }

    private void DOModifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.linearVelocity.x < 0) ||
                                  (direction.x < 0 && rb.linearVelocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.linearDamping = linearDrag;
            }
            else
            {
                rb.linearDamping = 0f;
            }

            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.linearDamping = linearDrag * 0.15f;
            if (rb.linearVelocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    void Flip()
    {
        _facingRight = !_facingRight;
        transform.rotation = Quaternion.Euler(0, _facingRight ? 0 : 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset,
            transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset,
            transform.position - colliderOffset + Vector3.down * groundLength);
    }
}