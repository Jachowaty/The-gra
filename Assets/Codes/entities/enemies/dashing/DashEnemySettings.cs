using UnityEngine;

[CreateAssetMenu(fileName = "DashEnemySettings", menuName = "Game/Enemy Settings/Dash")]
public class DashEnemySettings : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float acceleration = 40f;
    public float deceleration = 50f;

    [Header("Dash")]
    public float dashSpeed = 30f;
    public float dashRange = 6f;
    public float chargeTime = 0.5f;
    public float cooldownTime = 1.5f;
    public float shakeIntensity = 0.1f;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float attackTriggerRange = 5f;

    [Header("Combat")]
    public int damage = 1;
}