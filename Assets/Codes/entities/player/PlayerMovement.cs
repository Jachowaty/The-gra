using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 8.5f;
    public float acceleration = 60f;
    public float deceleration = 200f;
    private float currentSpeed;
    public Vector2 moveInput;

    [Header("Jumping")]
    public float jumpPower = 14f;
    public int maxJumps = 1;
    private int jumpsRemaining;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 3f;
    public float maxFallSpeed = 20f;
    public float fallSpeedMult = 2.5f;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.15f;
    public bool isKnockedBack;

    public bool IsGrounded { get; private set; }

    private PlayerSounds playerSounds;
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;
        Checkpoint.Initialize(startPosition);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        playerSounds = GetComponent<PlayerSounds>();
        GameController.OnReset += Respawn;
    }

    void OnDestroy()
    {
        GameController.OnReset -= Respawn;
    }

    void Update()
    {
        HandleAnimations();
        HandleFacing();
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;

        GroundCheck();
        HandleMovement();
        Gravity();
    }

    private void HandleMovement()
    {
        float targetSpeed = moveInput.x * moveSpeed;

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = 0f;
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    private void HandleFacing()
    {
        if (moveInput.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleAnimations()
    {
        if (IsGrounded)
        {
            animator.SetFloat("yVelocity", 0f);
        }
        else
        {
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
        }

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            animator.SetFloat("magnitude", Mathf.Abs(rb.linearVelocity.x));
        }
        else
        {
            animator.SetFloat("magnitude", 0f);
        }
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
                animator.SetTrigger("jump");
                if (playerSounds != null) playerSounds.PlayJumpSound();
            }
            else if (context.canceled && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
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
        StartCoroutine(KnockbackRoutine(sourcePosition, knockbackForce));
    }

    public void ApplyKnockback(Vector2 sourcePosition, float force)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(sourcePosition, force));
    }

    private IEnumerator KnockbackRoutine(Vector2 sourcePosition, float force)
    {
        isKnockedBack = true;
        animator.SetFloat("magnitude", 0);

        Vector2 direction = (transform.position - (Vector3)sourcePosition).normalized;
        Vector2 knockbackVector = new Vector2(Mathf.Sign(direction.x) * force, force * 0.5f);

        rb.linearVelocity = knockbackVector;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
        currentSpeed = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    void Respawn()
    {
        Vector3 pos = Checkpoint.GetSpawnPosition(startPosition);
        Debug.Log("Respawning at: " + pos);
        transform.position = pos;
        rb.linearVelocity = Vector2.zero;
        currentSpeed = 0f;
        jumpsRemaining = maxJumps;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}