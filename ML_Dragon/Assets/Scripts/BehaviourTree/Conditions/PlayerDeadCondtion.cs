using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class PlayerDiedNode : Node
    {
        private AgentState agentState;

        public PlayerDiedNode(AgentState agentState)
        {
            this.agentState = agentState;
        }

        public override NodeState Evaluate()
        {
            //Debug.Log($"Is player dead {health}");
            if (agentState.health<=0)
            {
                //Debug.Log("Yes dead");
                state = NodeState.SUCCESS;
            }
            else
            {
                //Debug.Log("Not dead");
                state = NodeState.FAILURE;
            }
            return state;
        }
    }
}
