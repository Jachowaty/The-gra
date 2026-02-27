using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public EnemySettings settings;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private float currentSpeed;
    private Rigidbody2D rb;
    private Transform player;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        startPos = transform.position;
        
        GameController.OnReset += ResetEnemy;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void OnDestroy()
    {
        GameController.OnReset -= ResetEnemy;
    }

    void ResetEnemy()
    {
        transform.position = startPos;
        gameObject.SetActive(true);
        rb.linearVelocity = Vector2.zero;
        currentSpeed = 0f;
        transform.localScale = new Vector3(1, 1, 1);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > settings.detectionRange)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, settings.deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
            return;
        }

        float distX = player.position.x - transform.position.x;
        float direction = Mathf.Sign(distX);

        if (Mathf.Abs(distX) > 0.5f)
        {
            float targetSpeed = direction * settings.moveSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, settings.acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

            if (Mathf.Abs(currentSpeed) > 0.1f)
            {
                transform.localScale = new Vector3(Mathf.Sign(currentSpeed), 1, 1);
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, settings.deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
        }

        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, settings.groundCheckDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(transform.position, Vector2.right * direction, settings.wallCheckDistance, wallLayer);

        if (wallAhead && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, settings.jumpForce);
        }
    }

    void OnDrawGizmos()
    {
        if (settings == null) return;

        Gizmos.color = Color.red;
        float dir = transform.localScale.x;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(dir, 0, 0) * settings.wallCheckDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * settings.groundCheckDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, settings.detectionRange);
    }
}