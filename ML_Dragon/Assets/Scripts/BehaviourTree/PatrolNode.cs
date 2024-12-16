using UnityEngine;
using UnityEngine.AI;
namespace BehaviorTree{
    public class PatrolNode : Node
    {
        private NavMeshAgent agent;
        private Animator animator;
        private Transform[] patrolPoints;
        private AgentState agentState;

        private bool hasStarted = false;

        public PatrolNode(NavMeshAgent agent, Animator animator,Transform[] patrolPoints, AgentState agentState)
        {
            this.agent = agent;
            this.animator = animator;
            this.patrolPoints = patrolPoints;
            this.agentState = agentState;
        }

        public override NodeState Evaluate()
        {
            if (patrolPoints.Length == 0)
            {
                state = NodeState.FAILURE;
                hasStarted = false;
                return state;
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                agentState.currentPatrolIndex = (agentState.currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[agentState.currentPatrolIndex].position);
            }
            
            animator.Play("Patrol");
            agent.speed = agentState.walkSpeed;
            state = NodeState.RUNNING;
            return state;
        }
    }
}
