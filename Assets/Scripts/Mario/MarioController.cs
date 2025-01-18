using System.Collections;
using UnityEngine;

public class MarioController : MonoBehaviour
{
    [Header("Horizontal Movement")] [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField] private Vector2 direction;
    private bool _facingRight = true;

    [Header("Vertical Movement")] [SerializeField]
    private float jumpSpeed = 15f;

    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Components")] [SerializeField]
    private Rigidbody2D rb;

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject characterHolder;

    [Header("Physics")] [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float linearDrag = 4f;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float fallMultiplier = 5f;

    [Header("Collision")] [SerializeField] private bool onGround = false;
    [SerializeField] private float groundLength = 0.6f;
    [SerializeField] private Vector3 colliderOffset;

    [SerializeField] private float rbVelocity;

    // Update is called once per frame
    void Update()
    {
        bool wasOnGround = onGround;
        onGround =
            Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
            Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        // if(!wasOnGround && onGround){
        // StartCoroutine(JumpSqueeze(1.25f, 0.8f, 0.05f));
        // }

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        animator.SetBool("onGround", onGround);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        rbVelocity = rb.linearVelocity.y;
        DOMoveCharacter(direction.x);
        if (jumpTimer > Time.time && onGround)
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

        animator.SetFloat("horizontal", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("vertical", rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0;
        // StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
    }

    private void DOModifyPhysics()
    {
        var changingDirections = (direction.x > 0 && rb.linearVelocity.x < 0) ||
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

    private void Flip()
    {
        _facingRight = !_facingRight;
        transform.rotation = Quaternion.Euler(0, _facingRight ? 0 : 180, 0);
    }

    private IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        var originalSize = Vector3.one;
        var newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        var t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }

        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }
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