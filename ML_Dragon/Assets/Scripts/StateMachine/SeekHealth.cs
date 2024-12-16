using UnityEngine.AI;
using UnityEngine;
using PathFinding;
namespace StateMachine{
    public class SeekHealthState : State
    {
        private Transform target;
        public SeekHealthState(StateMachine stateMachine, NavMeshAgent navMeshAgent,AgentController controller) : base(stateMachine, navMeshAgent,controller) { }

        public override void Enter(Transform target)
        {
            this.target = target;
            navMeshAgent.enabled = false;
            controller.StopAgent(false);
        }

        public override void Execute()
        {
            if(target == null){
                target = stateMachine.FindNearest<HealthTag>();
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