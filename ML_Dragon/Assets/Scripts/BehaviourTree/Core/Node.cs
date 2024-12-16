namespace BehaviorTree
{
    public abstract class Node
    {
        public enum NodeState
        {
            RUNNING,
            SUCCESS,
            FAILURE
        }

        protected NodeState state;

        public NodeState State { get { return state; } }

        public abstract NodeState Evaluate();
    }

}
