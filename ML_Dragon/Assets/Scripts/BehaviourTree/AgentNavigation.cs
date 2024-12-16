using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace BehaviorTree
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AgentVision))]
    [RequireComponent(typeof(AgentHearing))]
    [RequireComponent(typeof(AgentState))]
    public class AgentNavigation : BehaviorTree
    {
        public Transform[] patrolPoints;
        public Transform target;
        public Animator animator;
        private NavMeshAgent agent;
        private AgentVision vision;
        private AgentHearing hearing;
        private AgentState agentState;

        public float stuckThreshold = 0.1f; // Threshold to consider the agent as stuck
        public float checkInterval = 1f;   // Interval to check if the agent is stuck
        private Vector3 lastPosition;
        private float stuckTime;
        private float currentTime;
        public Transform startingPosition;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            vision = GetComponent<AgentVision>();
            hearing = GetComponent<AgentHearing>();
            agentState = GetComponent<AgentState>();

            lastPosition = transform.position;
            startingPosition = transform;
        }

        protected override Node SetupTree(){
            Node die = new DieNode(this, animator,agent, agentState);
            Node patrol = new PatrolNode(agent,animator, patrolPoints, agentState);
            Node chase = new ChaseNode(agent,animator,target,agentState);
            Node attack = new AttackNode(agent,animator,target,agentState);
            Node search = new SearchNode(agent,animator,agentState);

            Node playerDeadCondition = new PlayerDiedNode(agentState);
            Node playerSeenCondition = new PlayerSeenNode(vision);
            Node playerHeardCondition = new PlayerHeardNode(hearing);
            Node playerWasSeenCondition = new PlayerWasSeenNode(agentState);
            Node playerWithinChaseRangeCondition = new PlayerWithinChaseRange(agentState,target);

            return new Selector(new List<Node>
            {
                new Sequence(new List<Node> { playerDeadCondition, die}),
                new Sequence(new List<Node> 
                { 
                    new Selector(new List<Node> { playerSeenCondition, playerHeardCondition }),
                    new Selector(new List<Node> { attack,chase })
                }),
                new Sequence(new List<Node> 
                { 
                    playerWasSeenCondition,
                    search 
                }),
                patrol
            });
        }

        void Update(){
            base.Update();
            if (agent.isActiveAndEnabled)
            {
                // Check if the agent has been stuck for too long
                if (Time.time - currentTime >= checkInterval)
                {
                    if (Vector3.Distance(lastPosition, agent.transform.position) < stuckThreshold)
                    {
                        stuckTime += checkInterval;
                        if (stuckTime >= 10) // If stuck for 3 intervals
                        {
                            ResetAgentPosition();
                        }
                    }
                    else
                    {
                        stuckTime = 0; // Reset stuck time if moving
                    }

                    lastPosition = agent.transform.position;
                    currentTime = Time.time;
                }
            }
        }
        void ResetAgentPosition()
        {
            if (startingPosition != null)
            {
                agent.Warp(startingPosition.position);
                agent.ResetPath(); // Clear the current path
                Debug.Log("Agent reset to starting position");
            }
            else
            {
                //Debug.LogWarning("Starting position not set.");
            }
        }
    }

    
}

