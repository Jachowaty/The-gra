using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [Header("References")]
    public Transform swordPivot;
    public Rigidbody2D swordRb;
    public Collider2D swordCollider;

    [Header("Sword Settings")]
    public float swordDistance = 0.6f;
    public float groundCheckDistance = 2f;

    [Header("Damage Settings")]
    public int minDamage = 1;
    public int maxDamage = 5;
    public float minSwingSpeed = 5f;
    public float maxSwingSpeed = 30f;

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
    private Vector3 lastSwordPosition;
    private float currentSwingSpeed;

    void Start()
    {
        mainCamera = Camera.main;
        lastSwordPosition = swordPivot.position;
    }

    void Update()
    {
        MoveSwordToCursor();
        CalculateSwingSpeed();
    }

    void MoveSwordToCursor()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        Vector3 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector3 targetPosition = transform.position + direction * swordDistance;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, groundCheckDistance, solidLayers);

        if (hit.collider != null)
        {
            if (hit.distance < swordDistance)
            {
                targetPosition = (Vector3)hit.point - direction * 0.1f;

                if (direction.y < -0.3f)
                {
                    Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
                    if (playerRb.linearVelocity.y <= 0)
                    {
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
                        playerRb.gravityScale = 0f;
                    }
                }
            }
        }
        else
        {
            Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
            playerRb.gravityScale = GetComponent<PlayerMovement>().baseGravity;
        }

        swordPivot.rotation = Quaternion.Euler(0, 0, angle);
        swordPivot.position = targetPosition;
    }

    void CalculateSwingSpeed()
    {
        if (Time.deltaTime > 0)
        {
            float newSpeed = Vector3.Distance(swordPivot.position, lastSwordPosition) / Time.deltaTime;
            currentSwingSpeed = Mathf.Max(currentSwingSpeed * 0.9f, newSpeed);
        }
        lastSwordPosition = swordPivot.position;
    }

    public int GetDamage()
    {
        float speedPercent = Mathf.InverseLerp(minSwingSpeed, maxSwingSpeed, currentSwingSpeed);
        return Mathf.RoundToInt(Mathf.Lerp(minDamage, maxDamage, speedPercent));
    }

    public float GetSwingSpeed()
    {
        return currentSwingSpeed;
    }

    void OnDrawGizmos()
    {
        if (swordPivot == null) return;
        if (mainCamera == null) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;
        Vector3 direction = (mousePos - transform.position).normalized;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + direction * swordDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + direction * groundCheckDistance);
    }
}