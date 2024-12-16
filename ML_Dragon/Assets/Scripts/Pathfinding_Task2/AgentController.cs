using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    [RequireComponent(typeof(PathFindingAlgorithm))]
    [RequireComponent(typeof(PathFollower))]
    public class AgentController : MonoBehaviour
    {
        private PathFindingAlgorithm pathFinding;
        private PathFollower pathFollower;
        public Transform target;
        public float targetMoveSpeed = 5;
        public bool debugPath = false;
        private List<Node> path;

        void Awake()
        {
            pathFinding = GetComponent<PathFindingAlgorithm>();
            pathFollower = GetComponent<PathFollower>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                FindAndSetTargetHealthPack();
            }

            ControlTarget();
        }

        public void StopAgent(bool value){
            pathFollower.isMoving = !value;
        }

        public void FindAndSetTarget(Transform target){
            path = pathFinding.FindPath(transform.position, target.position);
            pathFollower.Follow(path);
        }
        public void FindAndSetTarget(Vector3 target){
            path = pathFinding.FindPath(transform.position, target);
            pathFollower.Follow(path);
        }

        private void FindAndSetTargetHealthPack()
        {
            HealthPack[] healthPacks = FindObjectsOfType<HealthPack>();
            if (healthPacks.Length == 0)
            {
                Debug.LogWarning("No health packs found in the scene.");
                return;
            }

            HealthPack closestHealthPack = null;
            float closestDistance = float.MaxValue;

            foreach (HealthPack healthPack in healthPacks)
            {
                float distance = Vector3.Distance(transform.position, healthPack.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHealthPack = healthPack;
                }
            }

            if (closestHealthPack != null)
            {
                target = closestHealthPack.transform;
                path = pathFinding.FindPath(transform.position, target.position);
                pathFollower.Follow(path);
            }
        }

        private void ControlTarget()
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                direction += Vector3.up;
            }
            if (Input.GetKey(KeyCode.RightControl))
            {
                direction += Vector3.down;
            }

            if(target != null){
                target.position += direction * targetMoveSpeed * Time.deltaTime;
            }
            
        }

        void OnDrawGizmos()
        {
            if (!debugPath)
            {
                return;
            }

            if (path != null)
            {
                foreach (Node node in path)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(node.worldPosition, .5f);
                }
            }
        }
    }
}
