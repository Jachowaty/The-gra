using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Game/Enemy Settings/Basic")]
public class EnemySettings : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float jumpForce = 8f;
    public float acceleration = 40f;
    public float deceleration = 50f;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float wallCheckDistance = 2.5f;
    public float groundCheckDistance = 1.5f;

    [Header("Combat")]
    public int damage = 1;
}