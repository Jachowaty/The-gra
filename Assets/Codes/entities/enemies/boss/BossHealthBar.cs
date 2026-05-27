using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Color fullHealthColor = Color.red;
    public Color lowHealthColor = new Color(0.5f, 0f, 0f);
    public float fadeSpeed = 2f;

    private Image[] allImages;
    private float currentAlpha;
    private float targetAlpha;

    void Start()
    {
        allImages = GetComponentsInChildren<Image>();
        currentAlpha = 0f;
        targetAlpha = 0f;
        SetAllAlpha(0f);
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetHealth(int health)
    {
        if (healthSlider == null) return;
        healthSlider.value = health;
    }

    public void FadeIn()
    {
        targetAlpha = 1f;
    }

    public void FadeOut()
    {
        targetAlpha = 0f;
    }

    void Update()
    {
        if (allImages == null) return;

        if (!Mathf.Approximately(currentAlpha, targetAlpha))
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            SetAllAlpha(currentAlpha);
        }
    }

    void SetAllAlpha(float alpha)
    {
        if (allImages == null) return;

        foreach (Image img in allImages)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }

    public void ResetHealthBar()
    {
        healthSlider.value = healthSlider.maxValue;
        currentAlpha = 0f;
        targetAlpha = 0f;
        SetAllAlpha(0f);
    }
}