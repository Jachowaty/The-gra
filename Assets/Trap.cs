using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public float BounceForce = 10f;
    public int Damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerBounce(collision.gameObject);
        }
    }
    private void HandlePlayerBounce(GameObject Player)
    {
        Rigidbody2D rb = Player.GetComponent<Rigidbody2D>();
        
        if(rb)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);

            rb.AddForce(Vector2.up * BounceForce, ForceMode2D.Impulse);
        }
    }
}
