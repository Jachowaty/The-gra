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
    private PlayerMovement playerMovement;
    private Camera mainCam;
    private Vector3 lastAttackDir; 
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        mainCam = Camera.main;
        lastAttackDir = Vector3.right;
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
            StartCoroutine(DoAttack());
    }
    
    IEnumerator DoAttack()
    {
        canAttack = false;
        
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = mainCam.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0f;

        Vector3 direction = (worldMousePos - transform.position).normalized;
        lastAttackDir = direction;

        Vector3 attackPos = transform.position + (direction * attackOffset);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (slashPrefab != null)
        {
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
        Vector3 drawDir = Application.isPlaying ? lastAttackDir : Vector3.right;
        Gizmos.DrawWireSphere(transform.position + (drawDir * attackOffset), attackRange);
    }
}