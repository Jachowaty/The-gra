using UnityEngine;
using System.Collections;

public class DashEnemy : MonoBehaviour
{
    public int damage = 1;
    public float moveSpeed = 3f;
    
    public float dashSpeed = 20f;
    public float dashRange = 6f; 
    public float attackTriggerRange = 5f; 
    public float detectionRange = 10f; 

    public float chargeTime = 0.5f;
    public float cooldownTime = 1.5f;
    public float shakeIntensity = 0.1f;
    
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private Transform player;
    private bool isAttacking = false;
    private float lastAttackTime;
    private Vector3 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        lastAttackTime = 0;
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (isAttacking) return;

        if (distToPlayer > detectionRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (distToPlayer <= attackTriggerRange && Time.time >= lastAttackTime + cooldownTime)
        {
            StartCoroutine(PerformDash());
        }
        else
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            
            if (distToPlayer > 1f)
            {
                rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
                transform.localScale = new Vector3(direction, 1, 1);
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    IEnumerator PerformDash()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = new Vector3(direction, 1, 1);

        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < chargeTime)
        {
            transform.position = startPos + (Vector3)Random.insideUnitCircle * shakeIntensity;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0);

        float calculatedDuration = dashRange / dashSpeed;
        yield return new WaitForSeconds(calculatedDuration);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1; 

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackTriggerRange);

        Gizmos.color = Color.blue;
        float dir = transform.localScale.x;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(dir * dashRange, 0, 0));
    }
}