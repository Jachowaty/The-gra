using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;
    private bool isDead;
    public BossHealthBar healthBar;
    public float healthBarRange = 15f;
    private bool healthBarVisible;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float retreatDistance = 3f;
    public float attackRange = 3f;
    public float acceleration = 40f;
    public float deceleration = 50f;
    private float currentSpeed;

    [Header("Attack")]
    public float attackCooldown = 3f;
    public int attackDamage = 1;
    public float attackKnockback = 25f;
    public float attackHitboxDelay = 0.3f;
    public float attackHitboxDuration = 0.2f;
    public GameObject attackHitbox;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking;
    private Transform player;
    private float lastAttackTime;
    private Vector3 startPos;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;

        Collider2D bossCollider = GetComponent<Collider2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj != null)
        {
            player = playerObj.transform;
            Collider2D playerCollider = playerObj.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(bossCollider, playerCollider);
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        ResetBoss();
        GameController.OnReset += ResetBoss;
    }

    void OnDestroy()
    {
        GameController.OnReset -= ResetBoss;
    }

    void ResetBoss()
    {
        StopAllCoroutines();

        currentHealth = maxHealth;
        isDead = false;
        isAttacking = false;
        healthBarVisible = false;
        currentSpeed = 0f;
        lastAttackTime = 0f;

        transform.position = startPos;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1;
        spriteRenderer.color = Color.white;

        GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(true);

        animator.SetBool("isAttacking", false);
        animator.Play("Boss_Idle");

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        if (healthBar != null)
        {
            healthBar.ResetHealthBar();
        }
    }

    void Update()
    {
        if (isDead) return;

        GroundCheck();
        UpdateAnimations();
        HandleMovement();
        HandleAttack();
        HandleHealthBar();
    }

    void HandleHealthBar()
    {
        if (healthBar == null) return;
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= healthBarRange && !healthBarVisible)
        {
            healthBarVisible = true;
            healthBar.FadeIn();
        }
        else if (distToPlayer > healthBarRange && healthBarVisible)
        {
            healthBarVisible = false;
            healthBar.FadeOut();
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    void UpdateAnimations()
    {
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("magnitude", Mathf.Abs(currentSpeed));
    }

    void HandleMovement()
    {
        if (player == null) return;
        if (isAttacking) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(direction * Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);

        if (distToPlayer < retreatDistance && Time.time < lastAttackTime + attackCooldown - 1f)
        {
            float targetSpeed = -direction * moveSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else if (distToPlayer > attackRange && Time.time >= lastAttackTime + attackCooldown - 1f)
        {
            float targetSpeed = direction * moveSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    void HandleAttack()
    {
        if (isAttacking) return;
        if (!isGrounded) return;
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (Time.time >= lastAttackTime + attackCooldown && distToPlayer <= attackRange)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(direction * Mathf.Abs(currentScale.x), currentScale.y, currentScale.z);

        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackHitboxDelay);

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
        }

        yield return new WaitForSeconds(attackHitboxDuration);

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(false);
        }

        yield return new WaitForSeconds(1f - attackHitboxDelay - attackHitboxDuration);

        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isAttacking) return;

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        StartCoroutine(HurtRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator HurtRoutine()
    {
        animator.SetTrigger("isHurt");
        StartCoroutine(FlashRed());
        yield return new WaitForSeconds(0.5f);
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead");
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        GetComponent<Collider2D>().enabled = false;

        if (healthBar != null)
        {
            healthBar.FadeOut();
        }

        // StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, retreatDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, healthBarRange);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}