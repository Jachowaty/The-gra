using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI endText;

    [Header("Settings")]
    public float fadeSpeed = 1f;
    public string winMessage = "Koniec";

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        HideScreen();
    }

    void Start()
    {
        GameController.OnReset += HideScreen;
    }

    void OnDestroy()
    {
        GameController.OnReset -= HideScreen;
    }

    public void ShowScreen()
    {
        endText.text = winMessage;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        StartCoroutine(FadeIn());
    }

    void HideScreen()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    System.Collections.IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}