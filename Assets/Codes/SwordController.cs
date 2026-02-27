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

        if (direction.x < 0)
        {
            swordSprite.flipX = true;
        }
        else
        {
            swordSprite.flipX = false;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

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
        
        SpriteRenderer sr = slash.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 50;
        }
    }
}