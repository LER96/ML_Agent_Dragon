using PathFinding;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine{
    public abstract class State
    {
        protected StateMachine stateMachine;
        protected NavMeshAgent navMeshAgent;
        protected AgentController controller;

        public State(StateMachine stateMachine, NavMeshAgent navMeshAgent, AgentController controller)
        {
            this.stateMachine = stateMachine;
            this.navMeshAgent = navMeshAgent;
            this.controller = controller;
        }

        public abstract void Enter(Transform target= null);
        public abstract void Execute();
        public abstract void Exit();
    }
}
