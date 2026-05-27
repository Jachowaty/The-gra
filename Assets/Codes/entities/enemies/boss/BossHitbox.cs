using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    private Boss boss;

    void Start()
    {
        boss = GetComponentInParent<Boss>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(boss.attackDamage);
            PlayerMovement movement = collision.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.ApplyKnockback(transform.position, boss.attackKnockback);
            }
        }
    }
}