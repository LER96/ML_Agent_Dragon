using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "ScriptableObjects/EnemySettings", order = 1)]
public class EnemySettings : ScriptableObject
{
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float sightRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 5f;
    public float searchDuration = 5f;
    public float alertRange = 15f;
}