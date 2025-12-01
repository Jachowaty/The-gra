using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float growSpeed = 15f;
    public float fadeSpeed = 8f;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 targetScale;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetScale = transform.localScale;
        transform.localScale = targetScale * 0.5f;
    }
    
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);
        
        Color color = spriteRenderer.color;
        color.a -= fadeSpeed * Time.deltaTime;
        spriteRenderer.color = color;
    }
}