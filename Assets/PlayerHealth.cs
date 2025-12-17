using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;
    private PlayerMovement playerMovement;

    public static event Action OnPlayerDeath;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        ResetHealth();
        GameController.OnReset += ResetHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Trap trap = collision.GetComponent<Trap>();
        EnemyProjectile projectile = collision.GetComponent<EnemyProjectile>();

        if(trap)
        {
            TakeDamage(trap.damage);
            playerMovement.ApplyKnockback(collision.transform.position);
        }

        if(projectile)
        {
            TakeDamage(projectile.damage);
            playerMovement.ApplyKnockback(collision.transform.position);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        DashEnemy dashEnemy = collision.gameObject.GetComponent<DashEnemy>(); 

        if(enemy)
        {
            TakeDamage(enemy.damage);
            playerMovement.ApplyKnockback(collision.transform.position);
        }
        else if(dashEnemy)
        {
            TakeDamage(dashEnemy.damage);
            playerMovement.ApplyKnockback(collision.transform.position);
        }
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        if(currentHealth <= 0 )
        {
            OnPlayerDeath.Invoke();
        }
    }
}