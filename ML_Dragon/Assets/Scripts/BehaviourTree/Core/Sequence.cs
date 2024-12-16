using System.Collections.Generic;
namespace BehaviorTree
{
    public class Sequence : Node
    {
        private List<Node> nodes = new List<Node>();

        public Sequence(List<Node> nodes)
        {
            this.nodes = nodes;
        }

        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;

            foreach (Node node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}
