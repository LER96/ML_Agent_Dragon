using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    [RequireComponent(typeof(Animator))]
    public class PathFollower : MonoBehaviour
    {
        public Transform target;
        public List<Node> path;
        public PathFindingAlgorithm aStar;
        public float speed = 5.0f;
        public float acceleration = 2.0f;
        public float turningRadius = 1.0f;
        public float stoppingDistance = 0.1f;
        public bool isGrounded = false;

        private Animator animator;
        private int currentNodeIndex = 0;
        private float currentSpeed;
        public bool isMoving = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Follow(List<Node> path)
        {
            this.path = path;
            isMoving = true;
            currentNodeIndex = 0;
            currentSpeed = speed;
        }

        void Update()
        {
            if (!isMoving)
            {
                animator.SetBool("IsMoving", isMoving);
                return;
            }
            if (path == null)
            {
                return;
            }
            if (currentNodeIndex >= path.Count)
            {
                isMoving = false;
                animator.SetBool("IsMoving", isMoving);
                return;
            }

            animator.SetBool("IsMoving", isMoving);

            Vector3 targetPosition = path[currentNodeIndex].worldPosition;
            Vector3 direction = (targetPosition - transform.position).normalized;

            if (isGrounded)
            {
                direction.y = 0; // Disregard movement in the Y axis if grounded
                targetPosition.y = transform.position.y; // Maintain current Y position if grounded
            }

            // Calculate steering
            float targetSpeed = speed;
            Vector3 desiredVelocity = direction * targetSpeed;
            Vector3 steering = desiredVelocity - (transform.forward * currentSpeed);
            steering = Vector3.ClampMagnitude(steering, acceleration * Time.deltaTime);

            // Update current speed and direction
            currentSpeed = Mathf.Clamp(currentSpeed + steering.magnitude, 0, speed);
            Vector3 newVelocity = transform.forward * currentSpeed + steering;
            transform.position += newVelocity * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turningRadius);

            if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
            {
                currentNodeIndex++;
            }
        }
    }
}
