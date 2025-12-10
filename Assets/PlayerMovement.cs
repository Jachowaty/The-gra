using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;

    public Vector2 moveInput;

    public float moveSpeed = 10f;
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMult = 2f;

    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    public bool isKnockedBack;

    public bool IsGrounded { get; private set; }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isKnockedBack) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        GroundCheck();
        Gravity();

        if (moveInput.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Gravity()
    {
        if(rb.linearVelocityY < 0 )
        {
            rb.gravityScale = baseGravity * fallSpeedMult;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }

        if (IsGrounded)
        {
            animator.SetFloat("yVelocity", 0f);
        }
        else
        {
            animator.SetFloat("yVelocity", rb.linearVelocityY);
        }

        animator.SetFloat("magnitude", Mathf.Abs(rb.linearVelocityX));
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0 )
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
                jumpsRemaining--;
                animator.SetTrigger("jump");
            }
            else if (context.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
            }
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    public void ApplyKnockback(Vector2 sourcePosition)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(sourcePosition));
    }

    private IEnumerator KnockbackRoutine(Vector2 sourcePosition)
    {
        isKnockedBack = true;
        animator.SetFloat("magnitude", 0); 
        
        Vector2 direction = (transform.position - (Vector3)sourcePosition).normalized;
        Vector2 knockbackVector = new Vector2(Mathf.Sign(direction.x) * knockbackForce, knockbackForce * 0.5f);
        
        rb.linearVelocity = knockbackVector;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero; 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}