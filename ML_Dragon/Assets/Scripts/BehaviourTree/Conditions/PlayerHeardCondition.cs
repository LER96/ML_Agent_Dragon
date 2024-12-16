using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class PlayerHeardNode : Node
    {
        private AgentHearing hearing;

        public PlayerHeardNode(AgentHearing hearing)
        {
            this.hearing = hearing;
        }

        public override NodeState Evaluate()
        {
            if (hearing.CanHearPlayer())
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
