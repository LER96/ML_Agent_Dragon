using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class SearchNode : Node
    {
        private NavMeshAgent agent;
        private Animator animator;
        private AgentState agentState;
        private float stayTimer;
        private bool animationSet;

        public SearchNode(NavMeshAgent agent, Animator animator, AgentState agentState)
        {
            this.agent = agent;
            this.animator = animator;
            this.agentState = agentState;
        }

        public override NodeState Evaluate()
        {
            agentState.isAttacking = false;
            if (agentState.searchTimer <= 0) 
            {
                EndSearch();
                return NodeState.SUCCESS;
            }
           

            agent.SetDestination(agentState.LastKnownPlayerPosition);
            

            if (HasReachedDestination())
            {
                animator.Play("Search");
                agentState.searchTimer -= Time.deltaTime;
                if (agentState.searchTimer <= 0)
                {
                    EndSearch();
                    return NodeState.SUCCESS;
                }
            }
            
            agent.speed = agentState.walkSpeed;
            return NodeState.RUNNING;
        }

        private bool HasReachedDestination()
        {
            return !agent.pathPending && agent.remainingDistance <= 1f;
        }

        private void EndSearch()
        {
            agentState.searchTimer = agentState.searchDuration;
            agentState.PlayerWasSeen = false;
            stayTimer = 0f;
            animationSet = false;
        }
    }
}
