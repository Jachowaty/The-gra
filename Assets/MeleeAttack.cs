using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttack : MonoBehaviour
{
    public float attackDamage = 1f;
    public float attackRange = 1f;
    public float attackCooldown = 0.4f;
    public float attackOffset = 0.8f;
    public LayerMask enemyLayers;
    public GameObject slashPrefab;
    
    private bool canAttack = true;
    private float direction = 1f;
    private PlayerMovement playerMovement;
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        if (playerMovement.rb.linearVelocityX > 0.1f)
            direction = 1f;
        else if (playerMovement.rb.linearVelocityX < -0.1f)
            direction = -1f;
        
        transform.localScale = new Vector3(direction, 1f, 1f);
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
            StartCoroutine(DoAttack());
    }
    
    IEnumerator DoAttack()
    {
        canAttack = false;
        
        Vector3 attackPos = transform.position + new Vector3(attackOffset * direction, 0f, 0f);
        
        if (slashPrefab != null)
        {
            float angle = direction > 0 ? 0f : 180f;
            Destroy(Instantiate(slashPrefab, attackPos, Quaternion.Euler(0, 0, angle)), 0.15f);
        }
        
        foreach (Collider2D enemy in Physics2D.OverlapCircleAll(attackPos, attackRange, enemyLayers))
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
        
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(attackOffset * direction, 0, 0), attackRange);
    }
}