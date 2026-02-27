using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float lifetime = 0.5f;
    public float growSpeed = 15f;
    public float fadeSpeed = 3f;
    public float moveSpeed = 3f;
    public int damage = 1;
    public LayerMask enemyLayers;

    private SpriteRenderer spriteRenderer;
    private Vector3 targetScale;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null) return;

        targetScale = transform.localScale;
        transform.localScale = targetScale * 0.3f;
        spriteRenderer.sortingOrder = 50;
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
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}