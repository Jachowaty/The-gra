using UnityEngine;
using System.Collections;

public class DashEnemy : MonoBehaviour
{
    public DashEnemySettings settings;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private float currentSpeed;
    private Rigidbody2D rb;
    private Transform player;
    private bool isAttacking = false;
    private float lastAttackTime;
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
        StopAllCoroutines();
        isAttacking = false;
        transform.position = startPos;
        gameObject.SetActive(true);
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1;
        currentSpeed = 0f;
        lastAttackTime = 0;
    }

    void FixedUpdate()
    {
        if (player == null) return;
        if (isAttacking) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer > settings.detectionRange)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, settings.deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
            return;
        }

        if (distToPlayer <= settings.attackTriggerRange && Time.time >= lastAttackTime + settings.cooldownTime)
        {
            StartCoroutine(PerformDash());
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        if (distToPlayer > 1f)
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
    }

    IEnumerator PerformDash()
    {
        isAttacking = true;
        currentSpeed = 0f;
        rb.linearVelocity = Vector2.zero;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = new Vector3(direction, 1, 1);

        float timer = 0f;
        Vector3 chargeStartPos = transform.position;

        while (timer < settings.chargeTime)
        {
            transform.position = chargeStartPos + (Vector3)Random.insideUnitCircle * settings.shakeIntensity;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = chargeStartPos;

        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(direction * settings.dashSpeed, 0);

        float calculatedDuration = settings.dashRange / settings.dashSpeed;
        yield return new WaitForSeconds(calculatedDuration);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1;

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    void OnDrawGizmos()
    {
        if (settings == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, settings.detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.attackTriggerRange);

        Gizmos.color = Color.blue;
        float dir = transform.localScale.x;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(dir * settings.dashRange, 0, 0));
    }
}