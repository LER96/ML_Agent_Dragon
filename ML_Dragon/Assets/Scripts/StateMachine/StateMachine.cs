using UnityEngine.AI;
using UnityEngine;
using PathFinding;
using System;

namespace StateMachine{
    [RequireComponent(typeof(FuzzyLogic))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AgentController))]
    public class StateMachine : MonoBehaviour
    {
        [HideInInspector] public Transform[] enemies;           // Enemies to avoid

        //public float lowHealthThreshold = 30f; // Threshold below which the agent seeks health
        public float safeDistance = 10f;       // Distance to maintain from enemies
        public float MaxDistanceToMonster = 20;
        public float[] weights = new float[3];

        private FuzzyLogic fuzzyLogic;
        private NavMeshAgent agent;
        private AgentController controller;
        private State currentState;
        public float health = 100f;

        private CollectCoin collectCoinState;
        private SeekHealthState seekHealthState;
        private AvoidState avoidState;

        private float triggerDistance = 5;
        private float damageAmount = 20;

        public float detectionRadius =2;
        public float fleeDistance = 2;

        //Fitness Score Parameters
        private float attackPunishment = 20;
        private float coinReward = 40;
        private float deathPunishment = 50;
        private float healthReward = 10;
        public float fitnessScore;

        private bool isReady = false;

        public float stuckThreshold = 0.1f; // Threshold to consider the agent as stuck
        public float checkInterval = 1f;   // Interval to check if the agent is stuck
        private Vector3 lastPosition;
        private float stuckTime;
        private float currentTime;
        public Transform startingPosition;

        public bool isGAEnabled = false;
        void OnEnable(){
            EventManager.Instance.events.onEnemyAttack.AddListener((enemyPostion)=>OnEnemyAttack(enemyPostion));
        }
        void OnDisable(){
            EventManager.Instance.events.onEnemyAttack.RemoveListener((enemyPostion)=>OnEnemyAttack(enemyPostion));
        }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            fuzzyLogic = GetComponent<FuzzyLogic>();
            controller = GetComponent<AgentController>();

            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            enemies = new Transform[enemyObjects.Length];
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                enemies[i] = enemyObjects[i].transform;
            }

            collectCoinState = new CollectCoin(this, agent,controller);
            seekHealthState = new SeekHealthState(this, agent,controller);
            avoidState = new AvoidState(this, agent,controller);

            lastPosition = transform.position;
            startingPosition = transform;

            if(!isGAEnabled){
                currentState = collectCoinState;
                currentState.Enter();
            }
            
            isReady = true;
            //ApplyGAParameters();
        }

        public void ApplyGAParameters(float[] parameters)
        {
            
            
            MaxDistanceToMonster =  parameters[0];
            weights[0] =  parameters[1];
            weights[1] =  parameters[2];
            weights[2] = parameters[3];
            safeDistance = parameters[4];
            detectionRadius = parameters[5];
            fleeDistance = parameters[6];

            fitnessScore = 0;
            health = 100;
            transform.position = new Vector3(10,0,30);
            currentState = collectCoinState;
            currentState.Enter();
            isReady = true;
        }

        void Update()
        {
            if(!isReady)
                return;
            currentState.Execute();
            MakeDecision();
        }

        void CheckStuck(){
            
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
               
            }
        }
        void MakeDecision()
        {
            float nearestDistance = FindNearestDistance(enemies);
            bool isChased = IsBeingChased();
             string behavior= "";
            if(isGAEnabled){
                behavior = fuzzyLogic.DetermineBehavior(health, nearestDistance, MaxDistanceToMonster, isChased, weights);
            }
            else{
                behavior = fuzzyLogic.DetermineBehavior(health, nearestDistance, MaxDistanceToMonster, isChased);
            }
            
            //Debug.Log($"CODE:{behavior}");
            switch (behavior)
            {
                case "Seeking Health":
                    if (!(currentState is SeekHealthState))
                    {
                        TransitionToState(seekHealthState);
                    }
                    break;

                case "Avoiding Monster":
                    if (!(currentState is AvoidState))
                    {
                        TransitionToState(avoidState);
                    }
                    break;

                case "Seeking Coins":
                    if (!(currentState is CollectCoin))
                    {
                        TransitionToState(collectCoinState);
                    }
                    break;
            }
        }

        void TransitionToState(State newState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter(FindNearest<HealthTag>());
        }

        public Transform FindNearest<T>() where T : MonoBehaviour
        {
        Transform nearestObject = null;
        float nearestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // Find all objects with the specified component in the scene
        T[] objects = GameObject.FindObjectsOfType<T>();

        foreach (T obj in objects)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestObject = obj.transform;
            }
        }

        return nearestObject;
        }

        public float FindNearestDistance(Transform[] objects)
        {
            float nearestDistance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (Transform obj in objects)
            {
                if (obj == null) continue;
                float distance = Vector3.Distance(currentPosition, obj.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                }
            }

            return nearestDistance;
        }

        private bool IsBeingChased()
        {
            foreach (Transform enemy in enemies)
            {
                //Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);
                if(currentState == avoidState && distanceToEnemy < safeDistance){
                    return true;
                }
                if (distanceToEnemy < detectionRadius)
                {
                    return true;
                }
            }
            return false; // No enemies in range or none within flee distance
        }

        private void OnEnemyAttack(Vector3 enemyPosition){
        float distanceToEnemy = Vector3.Distance(transform.position, enemyPosition);

            if (distanceToEnemy <= triggerDistance)
            {
                health -= damageAmount;
                if(health<=0){
                    fitnessScore -= deathPunishment;
                    EventManager.Instance.events.onRoundEnd.Invoke();
                    
                }
                else{
                    fitnessScore -= attackPunishment;
                }
                
            }
        }
        public void AddHealth(float amount){
            fitnessScore += healthReward * ((100 - health)/100f);
            health +=amount;
            health= Mathf.Clamp(health,-Mathf.Infinity,100);
        }
        public void AddCoinReward(){
            fitnessScore += coinReward;
        }
        public float GetFitnessScore(){
            return fitnessScore;
        }
    }
}