using UnityEngine;
using UnityEngine.AI;

public class AlertOthers
{
    private float alertRange;

    public AlertOthers(float alertRange)
    {
        this.alertRange = alertRange;
    }

    public void AlertNearbyEnemies(BaseEnemy enemy, Transform player, EnemySettings settings)
    {
        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, alertRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != enemy && hitCollider.gameObject.CompareTag("Enemy"))
            {
                EnemyBehaviorHandler otherEnemy = hitCollider.GetComponent<EnemyBehaviorHandler>();
                if (otherEnemy != null)
                {
                    otherEnemy.SetBehavior(new ChaseBehavior(otherEnemy.GetComponent<BaseEnemy>(), player, settings,otherEnemy.GetComponent<NavMeshAgent>(),otherEnemy.GetComponent<Animator>()));
                }
            }
        }
    }
}