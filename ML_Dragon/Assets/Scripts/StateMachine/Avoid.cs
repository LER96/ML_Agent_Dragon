using UnityEngine.AI;
using UnityEngine;
using PathFinding;
namespace StateMachine{
    public class AvoidState : State
    {
        private Transform[] enemies;
        private float detectionRadius;
        private float fleeDistance;

        public AvoidState(StateMachine stateMachine, NavMeshAgent navMeshAgent,AgentController controller) : base(stateMachine, navMeshAgent,controller) { 
            this.enemies = stateMachine.enemies;
            this.detectionRadius = stateMachine.detectionRadius;
            this.fleeDistance = stateMachine.fleeDistance;
        }

        public override void Enter(Transform target)
        {
            navMeshAgent.enabled = true;
            controller.StopAgent(true);
        }

        public override void Execute()
        {
    
            foreach (Transform enemy in enemies)
            {
                Vector3 fleeDirection = stateMachine.transform.position - enemy.position;
                Vector3 tentativeDestination = stateMachine.transform.position + fleeDirection.normalized * fleeDistance;

                // Ensure the new destination is a valid point on the NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(tentativeDestination, out hit, 5.0f, NavMesh.AllAreas))
                {
                    Vector3 validDestination = hit.position;
                    navMeshAgent.SetDestination(validDestination);
                }
                else
                {
                    Debug.LogWarning("No valid NavMesh position found near the intended destination.");
                }
            }
            
        }
    
        public override void Exit()
        {
           
        }
    }
}