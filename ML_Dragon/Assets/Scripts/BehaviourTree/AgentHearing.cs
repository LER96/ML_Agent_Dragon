using UnityEngine;

namespace BehaviorTree{
    public class AgentHearing : MonoBehaviour
    {
        public Transform player;
        public float hearingDistance = 10f;
        public LayerMask obstacleMask;
        public bool debugHearing = false;

        public bool CanHearPlayer()
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < hearingDistance)
            {
                return true;
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            if(!debugHearing){
                return;
            }
            
            Gizmos.color = CanHearPlayer()?Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, hearingDistance);
        }
    }
}

