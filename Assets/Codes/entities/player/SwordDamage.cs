using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    private SwordController swordController;

    [Header("Hit Settings")]
    public float minSpeedToDamage = 30f;
    public float hitCooldown = 0.3f;

    [Header("Hit Particles")]
    public Color hitParticleColor = Color.white;
    public int hitParticleCount = 3;
    public float hitParticleSpeed = 2f;
    public float hitParticleLifetime = 0.2f;
    public float hitParticleSize = 0.08f;

    private float lastHitTime;

    void Start()
    {
        swordController = FindObjectOfType<SwordController>();
        
        if (swordController == null)
        {
            Debug.LogError("SwordController not found!");
        }
        else
        {
            Debug.Log("SwordController found on: " + swordController.gameObject.name);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger hit: " + collision.gameObject.name);
        Debug.Log("Swing speed: " + swordController.GetSwingSpeed());
        Debug.Log("Min speed needed: " + minSpeedToDamage);

        if (Time.time < lastHitTime + hitCooldown) 
        {
            Debug.Log("On cooldown!");
            return;
        }

        float swingSpeed = swordController.GetSwingSpeed();
        if (swingSpeed < minSpeedToDamage)
        {
            Debug.Log("Too slow!");
            return;
        }

        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            int damage = swordController.GetDamage();
            Debug.Log("Dealing " + damage + " damage!");
            enemy.TakeDamage(damage);
            lastHitTime = Time.time;
            return;
        }

        Boss boss = collision.GetComponent<Boss>();
        if (boss != null)
        {
            int damage = swordController.GetDamage();
            Debug.Log("Dealing " + damage + " damage to boss!");
            boss.TakeDamage(damage);
            lastHitTime = Time.time;
            return;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time < lastHitTime + hitCooldown) return;

        Trap trap = collision.gameObject.GetComponent<Trap>();
        if (trap != null)
        {
            float swingSpeed = swordController.GetSwingSpeed();
            if (swingSpeed < minSpeedToDamage) return;

            BouncePlayer(collision.contacts[0].point);
            SpawnHitParticles(collision.contacts[0].point, collision.contacts[0].normal);
            lastHitTime = Time.time;
        }
    }

    void BouncePlayer(Vector3 trapPosition)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();
        Transform playerTransform = playerObj.transform;

        Vector2 bounceDirection = (playerTransform.position - trapPosition).normalized;

        if (Mathf.Abs(bounceDirection.y) < 0.3f)
        {
            bounceDirection.y = 1f;
            bounceDirection = bounceDirection.normalized;
        }

        float bounceForce = 15f;
        playerRb.linearVelocity = new Vector2(bounceDirection.x * bounceForce * 0.5f, Mathf.Abs(bounceDirection.y) * bounceForce);
    }

    void SpawnHitParticles(Vector2 hitPoint, Vector2 hitNormal)
    {
        for (int i = 0; i < hitParticleCount; i++)
        {
            GameObject particle = new GameObject("HitParticle");
            particle.transform.position = hitPoint;

            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSquareSprite();
            sr.color = hitParticleColor;
            sr.sortingOrder = 50;

            particle.transform.localScale = Vector3.one * hitParticleSize;

            Rigidbody2D rb = particle.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;

            float randomAngle = Random.Range(-45f, 45f);
            Vector2 particleDirection = Quaternion.Euler(0, 0, randomAngle) * hitNormal;
            rb.linearVelocity = particleDirection * hitParticleSpeed * Random.Range(0.5f, 1.5f);

            Destroy(particle, hitParticleLifetime);
        }
    }

    Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(4, 4);
        Color[] colors = new Color[16];
        for (int i = 0; i < 16; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
    }
}