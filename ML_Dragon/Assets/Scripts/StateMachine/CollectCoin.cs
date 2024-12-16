using PathFinding;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine{
    public class CollectCoin : State
    {
        private Transform target;
        public CollectCoin(StateMachine stateMachine, NavMeshAgent navMeshAgent,AgentController controller) : base(stateMachine, navMeshAgent,controller) { 

        }

        public override void Enter(Transform target)
        {
            this.target = target;
            navMeshAgent.enabled = false;
            //Debug.Log(controller);
            controller.StopAgent(false);
        }

        public override void Execute()
        {
            if(target == null){
                target = stateMachine.FindNearest<CoinTag>();
            }
            else{
                controller.FindAndSetTarget(target);
            }
        }

        public override void Exit()
        {
            target = null;
        }
    }
}