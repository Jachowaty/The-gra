using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float lifetime = 0.5f;
    public float growSpeed = 15f;
    public float fadeSpeed = 3f;
    public float moveSpeed = 3f;
    public int damage = 1;
    public float bounceForce = 15f;

    private SpriteRenderer spriteRenderer;
    private Vector3 targetScale;
    private float timer;
    private Transform player;
    private Rigidbody2D playerRb;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (spriteRenderer == null) return;

        targetScale = transform.localScale;
        transform.localScale = targetScale * 0.3f;
        spriteRenderer.sortingOrder = 50;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerRb = playerObj.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        timer += Time.deltaTime;

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);

        Color color = spriteRenderer.color;
        color.a -= fadeSpeed * Time.deltaTime;
        spriteRenderer.color = color;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

void OnTriggerEnter2D(Collider2D collision)
{
    Debug.Log("Slash hit: " + collision.gameObject.name);

    EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
    if (enemy != null)
    {
        Debug.Log("Hit enemy!");
        enemy.TakeDamage(damage);
        return;
    }

    Trap trap = collision.GetComponent<Trap>();
    if (trap != null)
    {
        Debug.Log("Hit trap!");
        if (playerRb != null)
        {
            BouncePlayer(collision.transform.position);
        }
        else
        {
            Debug.Log("No player rigidbody!");
        }
    }
}

    void BouncePlayer(Vector3 trapPosition)
    {
        Vector2 bounceDirection = (player.position - trapPosition).normalized;
        
        if (Mathf.Abs(bounceDirection.y) < 0.3f)
        {
            bounceDirection.y = 1f;
            bounceDirection = bounceDirection.normalized;
        }
        
        playerRb.linearVelocity = new Vector2(bounceDirection.x * bounceForce * 0.5f, Mathf.Abs(bounceDirection.y) * bounceForce);
    }
}