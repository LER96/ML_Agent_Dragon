using UnityEngine;
using System.Collections;

public class FlyAgent : BaseAgent
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private  Rigidbody rb;

    protected override void Start()
    {
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
        StartCoroutine(FollowPath());
    }

    protected override void MoveToNextWaypoint()
    {
        
    }

    private IEnumerator FollowPath()
    {
        if (waypoints == null || waypoints.Length == 0)
            yield break;

        Vector3 currentWaypoint = waypoints[targetIndex].position;

        while (true)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) < 0.5f)
            {
                targetIndex = (targetIndex + 1) % waypoints.Length;
                currentWaypoint = waypoints[targetIndex].position;
            }

            Vector3 direction = (currentWaypoint - transform.position).normalized;
            Vector3 velocity = direction * speed;
            rb.velocity = velocity;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);

            // Ensure the agent stays in the air
            if (Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer))
            {
                rb.AddForce(Vector3.up * 10f); // Adjust the force as needed
            }

            yield return null;
        }
    }
}