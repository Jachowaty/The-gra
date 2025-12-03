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
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());
        
        if (currentHealth <= 0)
            Destroy(gameObject);
    }
    
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (currentHealth > 0)
            spriteRenderer.color = originalColor;
    }
}