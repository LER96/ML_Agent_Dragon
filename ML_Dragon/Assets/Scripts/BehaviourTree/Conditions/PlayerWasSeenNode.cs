using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class PlayerWasSeenNode : Node
    {
        private AgentState agentState;

        public PlayerWasSeenNode(AgentState agentState)
        {
            this.agentState = agentState;
        }

        public override NodeState Evaluate()
        {
            if (agentState.PlayerWasSeen)
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
