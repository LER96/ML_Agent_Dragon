using UnityEngine;

namespace BehaviorTree{
    public class AgentVision : MonoBehaviour
    {
        public Transform player;
        public float viewDistance = 10f;
        public float viewAngle = 120f;
        public LayerMask obstacleMask;
        public bool debugVision = false;

        public bool CanSeePlayer()
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < viewDistance)
            {
                float angleBetweenEnemyAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                if (angleBetweenEnemyAndPlayer < viewAngle / 2f)
                {
                    if (!Physics.Linecast(transform.position, player.position, obstacleMask))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            if(!debugVision){
                return;
            }
            Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, viewDistance);

            Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewDistance);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewDistance);

            if (CanSeePlayer())
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}

