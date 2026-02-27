using UnityEngine;

[CreateAssetMenu(fileName = "ShootingEnemySettings", menuName = "Game/Enemy Settings/Shooting")]
public class ShootingEnemySettings : ScriptableObject
{
    [Header("Detection")]
    public float detectionRange = 10f;

    [Header("Combat")]
    public float fireRate = 2f;
    public Vector2 shootOffset = new Vector2(0f, 0.5f);
}