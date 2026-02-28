using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [Header("References")]
    public Transform swordPivot;
    public SpriteRenderer swordSprite;
    public GameObject slashEffectPrefab;

    [Header("Sword Settings")]
    public float swordDistance = 0.6f;
    public float slashDistance = 1.2f;
    public float swingDuration = 0.15f;
    public float swingAngle = 90f;

    [Header("Attack Settings")]
    public float attackCooldown = 0.3f;
    public int damage = 1;

    [Header("Wall Detection")]
    public LayerMask solidLayers;
    public float wallCheckDistance = 1.5f;
    public float raySpreadAngle = 30f;
    public int rayCount = 5;

    [Header("Wall Particles")]
    public Color particleColor = Color.white;
    public int particleCount = 5;
    public float particleSpeed = 3f;
    public float particleLifetime = 0.3f;
    public float particleSize = 0.1f;

    private Camera mainCamera;
    private float lastAttackTime;
    private bool isSwinging;
    private float swingTimer;
    private float startAngle;
    private int swingDirection;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isSwinging)
        {
            AimAtCursor();
        }
        else
        {
            swordPivot.position = transform.position + (swordPivot.position - transform.position).normalized * swordDistance;
            PerformSwing();
        }
    }

    void AimAtCursor()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector3 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        swordPivot.rotation = Quaternion.Euler(0, 0, angle);
        swordPivot.position = transform.position + direction * swordDistance;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;
        Vector3 direction = (mousePos - transform.position).normalized;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = Mathf.Lerp(-raySpreadAngle, raySpreadAngle, (float)i / (rayCount - 1));
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * direction;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, wallCheckDistance, solidLayers);

            if (hit.collider != null)
            {
                SpawnWallParticles(hit.point, hit.normal);
                lastAttackTime = Time.time;
                return;
            }
        }

        StartSwing();
    }

    void StartSwing()
    {
        isSwinging = true;
        swingTimer = 0f;
        lastAttackTime = Time.time;

        startAngle = swordPivot.rotation.eulerAngles.z;
        swingDirection = Random.value > 0.5f ? 1 : -1;

        SpawnSlashEffect();
    }

    void PerformSwing()
    {
        swingTimer += Time.deltaTime;
        float progress = swingTimer / swingDuration;

        float swingCurve = Mathf.Sin(progress * Mathf.PI);
        float currentAngle = startAngle + (swingAngle * swingCurve * swingDirection);

        swordPivot.rotation = Quaternion.Euler(0, 0, currentAngle);

        if (progress >= 1f)
        {
            isSwinging = false;
        }
    }

    void SpawnSlashEffect()
    {
        if (slashEffectPrefab == null) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;
        Vector3 direction = (mousePos - transform.position).normalized;
        Vector3 spawnPos = transform.position + direction * slashDistance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject slash = Instantiate(slashEffectPrefab, spawnPos, Quaternion.Euler(0, 0, angle));
        
        SlashEffect slashEffect = slash.GetComponent<SlashEffect>();
        if (slashEffect != null)
        {
            slashEffect.damage = damage;
        }

        SpriteRenderer sr = slash.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 50;
        }
    }

    void SpawnWallParticles(Vector2 hitPoint, Vector2 hitNormal)
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = new GameObject("WallParticle");
            particle.transform.position = hitPoint;

            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSquareSprite();
            sr.color = particleColor;
            sr.sortingOrder = 50;

            particle.transform.localScale = Vector3.one * particleSize;

            Rigidbody2D rb = particle.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;

            float randomAngle = Random.Range(-45f, 45f);
            Vector2 particleDirection = Quaternion.Euler(0, 0, randomAngle) * hitNormal;
            rb.linearVelocity = particleDirection * particleSpeed * Random.Range(0.5f, 1.5f);

            Destroy(particle, particleLifetime);
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

    void OnDrawGizmos()
    {
        if (swordPivot == null) return;

        Vector3 direction = swordPivot.right;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = Mathf.Lerp(-raySpreadAngle, raySpreadAngle, (float)i / (rayCount - 1));
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * direction;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, wallCheckDistance, solidLayers);

            if (hit.collider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.1f);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + rayDirection * wallCheckDistance);
            }
        }
    }
}