using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree{
    public class PlayerWithinChaseRange : Node
    {
        private AgentState agentState;
        private Transform player;
        public PlayerWithinChaseRange(AgentState agentState,Transform player)
        {
            this.agentState = agentState;
            this.player = player;
        }

        public override NodeState Evaluate()
        {
            float distanceToPlayer = Vector3.Distance(agentState.transform.position, player.position);
            if (distanceToPlayer <= agentState.attackRange)
            {
                state = NodeState.SUCCESS;
            }
            else
            {
                state = NodeState.FAILURE;
            }
            return state;
        }
    }
}

