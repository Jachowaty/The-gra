using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    public ShootingEnemySettings settings;
    public GameObject projectilePrefab;

    private Transform player;
    private float nextFireTime;
    private Vector3 startPos;

    void Start()
    {
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
        transform.position = startPos;
        gameObject.SetActive(true);
        nextFireTime = Time.time + 1f;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= settings.detectionRange)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            transform.localScale = new Vector3(direction, 1, 1);

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + settings.fireRate;
            }
        }
    }

    void Shoot()
    {
        Vector3 spawnPos = transform.position + (Vector3)settings.shootOffset;
        Vector3 direction = (player.position - spawnPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject newProjectile = Instantiate(projectilePrefab, spawnPos, Quaternion.Euler(0, 0, angle));
        newProjectile.transform.SetParent(transform);
    }

    void OnDrawGizmosSelected()
    {
        if (settings == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, settings.detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + (Vector3)settings.shootOffset, 0.2f);
    }
}