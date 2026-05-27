using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource backgroundMusic;
    public AudioSource bossMusic;

    [Header("Clips")]
    public AudioClip backgroundClip;
    public AudioClip bossClip;

    [Header("Settings")]
    public float fadeSpeed = 1f;
    public float slowFadeSpeed = 0.05f;
    public float backgroundVolume = 0.3f;
    public float bossVolume = 0.4f;

    private bool bossMusicActive;
    private float bgTarget;
    private float bossTarget;
    private float currentFadeSpeed;

    void Start()
    {
        backgroundMusic.clip = backgroundClip;
        backgroundMusic.loop = true;
        backgroundMusic.volume = backgroundVolume;
        backgroundMusic.Play();

        bossMusic.clip = bossClip;
        bossMusic.loop = true;
        bossMusic.volume = 0f;
        bossMusic.Play();

        bgTarget = backgroundVolume;
        bossTarget = 0f;
        currentFadeSpeed = fadeSpeed;

        GameController.OnReset += StopBossMusic;
    }

    void OnDestroy()
    {
        GameController.OnReset -= StopBossMusic;
    }

    void Update()
    {
        backgroundMusic.volume = Mathf.MoveTowards(backgroundMusic.volume, bgTarget, currentFadeSpeed * Time.deltaTime);
        bossMusic.volume = Mathf.MoveTowards(bossMusic.volume, bossTarget, currentFadeSpeed * Time.deltaTime);
    }

    public void StartBossMusic()
    {
        if (bossMusicActive) return;
        bossMusicActive = true;
        currentFadeSpeed = fadeSpeed;
        bgTarget = 0f;
        bossTarget = bossVolume;
    }

    public void StopBossMusic()
    {
        bossMusicActive = false;
        currentFadeSpeed = fadeSpeed;
        bgTarget = backgroundVolume;
        bossTarget = 0f;
    }

    public void SlowFadeBossMusic()
    {
        bossMusicActive = false;
        currentFadeSpeed = slowFadeSpeed;
        bossTarget = 0f;
        bgTarget = 0f;
    }
}