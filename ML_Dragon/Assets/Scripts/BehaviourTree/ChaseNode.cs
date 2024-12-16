using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree{
    public class ChaseNode : Node
    {
        private NavMeshAgent agent;
        private Animator animator;
        private Transform player;
        private AgentState agentState;

        public ChaseNode(NavMeshAgent agent, Animator animator, Transform player, AgentState agentState)
        {
            this.agent = agent;
            this.animator = animator;
            this.player = player;
            this.agentState = agentState;
        }

        public override NodeState Evaluate()
        {
            float distanceToPlayer = Vector3.Distance(agent.transform.position, player.position);
            if (distanceToPlayer <= agentState.chaseRange)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(player.position, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                    agentState.LastKnownPlayerPosition = hit.position;
                    state = NodeState.RUNNING;
                }
                else
                {
                    state = NodeState.FAILURE;
                }
            }
            else
            {
                state = NodeState.FAILURE;
            }

            animator.Play("Chase");
            agent.speed = agentState.chaseSpeed;
            agentState.PlayerWasSeen = true;
            agentState.searchTimer = 6;
            return state;
        }
    }
}
