using UnityEngine;

public class platformer_movements : MonoBehaviour
{
    [Header("Mouvements")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Touches")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool facingRight = true;

    private bool isDashing = false;
    private bool canDash = true;
    private float originalGravity;

    private bool isKnockedback = false;

    public Animator CharacterAnimator;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    // Touches + Animators
    void Update()
    {
        if (isDashing || isKnockedback)
            return;

        float moveDirection = 0f;

        if (Input.GetKey(leftKey))
        {
            moveDirection = -1f;
        }
        else if (Input.GetKey(rightKey))
        {
            moveDirection = 1f;
        }

        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        if (moveDirection > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveDirection < 0 && facingRight)
        {
            Flip();
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        if (Input.GetKeyDown(dashKey) && canDash)
        {
            StartCoroutine(DoDash());
        }

        CharacterAnimator.SetFloat("Speed2", Mathf.Abs(rb.linearVelocity.x));
        CharacterAnimator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        if (GetComponent<Rigidbody2D>().linearVelocity.y != 0)
             {
             CharacterAnimator.SetBool("IsJumping2", true);
             }
        else
             {
             CharacterAnimator.SetBool("IsJumping2", false);
             }


    }


    void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    // Dash
    System.Collections.IEnumerator DoDash()
    {
        isDashing = true;
        canDash = false;

        float dashDirection = facingRight ? 1f : -1f;

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Knockback(float duration)
    {
        StartCoroutine(KnockbackRoutine(duration));
    }

    private System.Collections.IEnumerator KnockbackRoutine(float duration)
    {
        isKnockedback = true;
        yield return new WaitForSeconds(duration);
        isKnockedback = false;
    }

}
