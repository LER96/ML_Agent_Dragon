using UnityEngine;

namespace BehaviorTree{
    public class AgentState : MonoBehaviour
    {
            public float walkSpeed = 2.5f;
            public float chaseSpeed = 4.5f;
            public int health = 100;
            public float chaseRange = 10f;
            public float attackRange = 2f;
            public float attackCooldown = 2f;
            public float searchDuration = 5f;
            public float searchTimer =10;
            public bool isAttacking = false;
            [SerializeField] public float attackTimer = 0;

            [SerializeField] public bool PlayerWasSeen = false;
            [SerializeField] public Vector3 LastKnownPlayerPosition;
            [SerializeField] public int currentPatrolIndex;
            [SerializeField] public float deathTimer = 8;

            void Update(){
                if(Input.GetKeyDown(KeyCode.D)){
                    health = 0;
                }
            }
    }
}
