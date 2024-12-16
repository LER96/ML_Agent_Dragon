using UnityEngine;

using BehaviorTree;
public class Bomb : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        AgentState agentState = other.GetComponent<AgentState>();
        if (agentState != null)
        {
            agentState.health = 0;
        }
    }
}
