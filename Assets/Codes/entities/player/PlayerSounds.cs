using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Walk Clips")]
    public AudioClip walkClip1;
    public AudioClip walkClip2;
    public AudioClip walkClip3;

    [Header("Other Clips")]
    public AudioClip jumpClip;
    public AudioClip hurtClip;

    [Header("Settings")]
    public float walkVolume = 0.5f;
    public float jumpVolume = 0.7f;
    public float hurtVolume = 0.8f;
    public float walkStepInterval = 0.35f;

    private AudioClip[] walkClips;
    private PlayerMovement playerMovement;
    private float stepTimer;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        walkClips = new AudioClip[] { walkClip1, walkClip2, walkClip3 };
    }

    void Update()
    {
        HandleWalkSound();
    }

    void HandleWalkSound()
    {
        bool isMoving = Mathf.Abs(playerMovement.moveInput.x) > 0.1f;
        bool isGrounded = playerMovement.IsGrounded;

        if (isMoving && isGrounded)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                AudioClip clip = walkClips[Random.Range(0, walkClips.Length)];
                PlaySound(clip, walkVolume, Random.Range(0.9f, 1.1f));
                stepTimer = walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void PlayJumpSound()
    {
        PlaySound(jumpClip, jumpVolume);
    }

    public void PlayHurtSound()
    {
        PlaySound(hurtClip, hurtVolume);
    }

    void PlaySound(AudioClip clip, float volume, float pitch = 1f)
    {
        if (clip == null) return;

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
    }
}