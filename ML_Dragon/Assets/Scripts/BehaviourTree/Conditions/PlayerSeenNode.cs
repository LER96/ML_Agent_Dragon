using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class PlayerSeenNode : Node
    {
        private AgentVision vision;

        public PlayerSeenNode(AgentVision vision)
        {
            this.vision = vision;
        }

        public override NodeState Evaluate()
        {
            if (vision.CanSeePlayer())
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
