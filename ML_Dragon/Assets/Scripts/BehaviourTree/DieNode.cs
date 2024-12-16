using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree{
    public class DieNode : Node
    {
        private MonoBehaviour enemy;
        private NavMeshAgent agent;
        private Animator animator;
        private AgentState agentState;

        public DieNode(MonoBehaviour enemy, Animator animator, NavMeshAgent agent, AgentState agentState)
        {
            this.enemy = enemy;
            this.agent = agent;
            this.animator = animator;
            this.agentState = agentState;
        }

        public override NodeState Evaluate()
        {
            if(agentState.deathTimer <=0){
                
            enemy.gameObject.SetActive(false);
            state = NodeState.SUCCESS;
            return state;
            }
            agent.ResetPath();
            animator.Play("Die");
            agentState.deathTimer -= Time.deltaTime;
            return state;
        }
    }
}
