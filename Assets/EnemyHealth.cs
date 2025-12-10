using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 3f;
    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        
        GameController.OnReset += ResetEnemyHealth;
    }

    void OnDestroy()
    {
        GameController.OnReset -= ResetEnemyHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void ResetEnemyHealth()
    {
        currentHealth = maxHealth;
        spriteRenderer.color = originalColor;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (currentHealth > 0)
            spriteRenderer.color = originalColor;
    }
}