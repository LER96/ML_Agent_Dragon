namespace BehaviorTree
{
    public class Inverter : Node
    {
        private Node node;

        public Inverter(Node node)
        {
            this.node = node;
        }

        public override NodeState Evaluate()
        {
            switch (node.Evaluate())
            {
                case NodeState.SUCCESS:
                    state = NodeState.FAILURE;
                    break;
                case NodeState.FAILURE:
                    state = NodeState.SUCCESS;
                    break;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    break;
                default:
                    state = NodeState.FAILURE;
                    break;
            }
            return state;
        }
    }
}
