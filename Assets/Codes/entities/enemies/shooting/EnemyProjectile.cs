using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 5f;
    public float hoverTime = 0.5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(LaunchSequence());
    }

    IEnumerator LaunchSequence()
    {
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(hoverTime);

        transform.SetParent(null);
        rb.linearVelocity = transform.right * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Ground") || 
            collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }
}