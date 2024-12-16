using UnityEngine;
using UnityEngine.AI;

namespace BehaviorTree
{
    public class AttackNode : Node
    {
        private NavMeshAgent agent;
        private Animator animator;
        private Transform player;
        private AgentState agentState;


        public AttackNode(NavMeshAgent agent, Animator animator, Transform player, AgentState agentState)
        {
            this.agent = agent;
            this.animator = animator;
            this.player = player;
            this.agentState = agentState;
        }

        public override NodeState Evaluate()
        {
            if(agentState.isAttacking){
               
                animator.Play("Attack");
                state = NodeState.RUNNING;
                agentState.attackTimer +=Time.deltaTime;
                 if (agentState.attackTimer >= agentState.attackCooldown)
                    {
                        agentState.attackTimer =0;
                        agentState.isAttacking = false;
                        state = NodeState.SUCCESS;
                    }
            }
            else{
                
                float distanceToPlayer = Vector3.Distance(agent.transform.position, player.position);
                if (distanceToPlayer <= agentState.attackRange)
                {
                    agentState.isAttacking= true;
                    EventManager.Instance.events.onEnemyAttack.Invoke(agent.transform.position);
                    state = NodeState.RUNNING;
                }
                else
                {
                    agentState.attackTimer =0;
                    agentState.isAttacking = false;
                    state = NodeState.FAILURE; 
                }
            }
            return state;
        }
    }
}
