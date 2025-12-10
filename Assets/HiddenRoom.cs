using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRoom : MonoBehaviour
{
    public Tilemap coverTilemap;
    public SpriteRenderer coverSprite;

    public float fadeSpeed = 5f;
    public float transparentAlpha = 0f;

    private float targetAlpha = 1f;
    private Color originalColor;

    void Start()
    {
        if (coverTilemap != null) originalColor = coverTilemap.color;
        else if (coverSprite != null) originalColor = coverSprite.color;
    }

    void Update()
    {
        float currentAlpha;

        if (coverTilemap != null)
        {
            currentAlpha = coverTilemap.color.a;
            float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            coverTilemap.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
        }
        else if (coverSprite != null)
        {
            currentAlpha = coverSprite.color.a;
            float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            coverSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetAlpha = transparentAlpha;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            targetAlpha = 1f;
        }
    }
}