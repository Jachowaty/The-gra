using UnityEngine;
using UnityEngine.InputSystem;

public class SwordController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D swordRb;
    public Rigidbody2D playerRb;

    [Header("Sword Movement")]
    public float maxRange = 2f;
    public float positionForce = 50f;
    public float maxSpeed = 20f;
    public float stopDrag = 10f;
    public float moveDrag = 2f;

    [Header("Sword Rotation")]
    public float turnSpeed = 360f;
    public float spriteAngleOffset = -90f;

    [Header("Damage Settings")]
    public int minDamage = 1;
    public int maxDamage = 3;
    public float minSwingSpeed = 30f;
    public float maxSwingSpeed = 150f;

    private Camera mainCamera;
    private float currentSwingSpeed;
    private Vector2 lastTipPosition;
    private Vector2 targetPosition;
    private float targetAngle;

    void Start()
    {
        if (swordRb == null)
        {
            swordRb = GetComponent<Rigidbody2D>();
        }

        if (playerRb == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerRb = playerObj.GetComponent<Rigidbody2D>();
            }
        }

        mainCamera = Camera.main;
        lastTipPosition = swordRb.position;

        IgnorePlayerCollisions();
        GameController.OnReset += ResetSword;
    }

    void OnDestroy()
    {
        GameController.OnReset -= ResetSword;
    }

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        if (Mouse.current == null) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector2 toMouse = (Vector2)mouseWorld - playerRb.position;

        if (toMouse.magnitude > maxRange)
        {
            targetPosition = playerRb.position + toMouse.normalized * maxRange;
        }
        else
        {
            targetPosition = mouseWorld;
        }

        Vector2 moveDirection = targetPosition - swordRb.position;
        if (moveDirection.magnitude > 0.1f)
        {
            targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg + spriteAngleOffset;
        }

        CalculateSwingSpeed();
    }

    void FixedUpdate()
    {
        if (playerRb == null) return;

        MoveSword();
        RotateSword();
        ClampVelocity();
    }

    void MoveSword()
    {
        Vector2 toTarget = targetPosition - swordRb.position;
        float distance = toTarget.magnitude;

        if (distance < 0.05f)
        {
            swordRb.linearDamping = stopDrag;
            return;
        }

        swordRb.linearDamping = moveDrag;
        swordRb.AddForce(toTarget.normalized * positionForce * distance, ForceMode2D.Force);
    }

    void RotateSword()
    {
        float currentAngle = swordRb.rotation;
        float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
        float maxStep = turnSpeed * Time.fixedDeltaTime;
        float newAngle = currentAngle + Mathf.Clamp(angleDiff, -maxStep, maxStep);

        swordRb.MoveRotation(newAngle);
        swordRb.angularVelocity = 0f;
    }

    void ClampVelocity()
    {
        swordRb.linearVelocity = Vector2.ClampMagnitude(swordRb.linearVelocity, maxSpeed);
    }

    void IgnorePlayerCollisions()
    {
        if (playerRb == null) return;

        Collider2D[] playerColliders = playerRb.GetComponents<Collider2D>();
        Collider2D[] swordColliders = GetComponents<Collider2D>();

        for (int i = 0; i < playerColliders.Length; i++)
        {
            for (int j = 0; j < swordColliders.Length; j++)
            {
                Physics2D.IgnoreCollision(playerColliders[i], swordColliders[j], true);
            }
        }
    }

    void ResetSword()
    {
        StartCoroutine(ResetSwordDelayed());
    }

    System.Collections.IEnumerator ResetSwordDelayed()
    {
        yield return new WaitForSeconds(0.1f);

        swordRb.linearVelocity = Vector2.zero;
        swordRb.angularVelocity = 0f;
        swordRb.position = playerRb.position + Vector2.right * maxRange * 0.5f;
        swordRb.rotation = spriteAngleOffset;
        currentSwingSpeed = 0f;
        lastTipPosition = swordRb.position;
    }

    void CalculateSwingSpeed()
    {
        Vector2 tipPosition = swordRb.position;

        if (Time.deltaTime > 0f)
        {
            float newSpeed = Vector2.Distance(tipPosition, lastTipPosition) / Time.deltaTime;
            currentSwingSpeed = Mathf.Max(currentSwingSpeed * 0.9f, newSpeed);
        }

        lastTipPosition = tipPosition;
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

    void OnDrawGizmosSelected()
    {
        if (playerRb == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(playerRb.position, maxRange);
    }
}   