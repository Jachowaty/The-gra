using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Vector3 spawnPosition;
    private static bool hasCheckpoint;

    public static void Initialize(Vector3 defaultPosition)
    {
        if (!hasCheckpoint)
        {
            spawnPosition = defaultPosition;
        }
    }

    public static Vector3 GetSpawnPosition(Vector3 fallback)
    {
        if (spawnPosition == Vector3.zero && !hasCheckpoint)
        {
            return fallback;
        }
        return spawnPosition;
    }

    public static void ResetCheckpoints()
    {
        hasCheckpoint = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spawnPosition = transform.position;
            hasCheckpoint = true;
        }
    }
}