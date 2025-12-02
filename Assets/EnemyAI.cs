using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpForce = 8f;
    public float wallCheckDistance = 2.5f;
    public float groundCheckDistance = 1.5f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = player.position.x - transform.position.x;
        float direction = Mathf.Sign(distToPlayer);

        if (Mathf.Abs(distToPlayer) > 0.5f)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(direction, 1, 1);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(transform.position, Vector2.right * direction, wallCheckDistance, wallLayer);

        if (wallAhead && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float dir = transform.localScale.x;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(dir, 0, 0) * wallCheckDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}