using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class PathFollower3D : MonoBehaviour
{
    public WaypointHandler waypointHandler;
    public float speed = 5f;
    public float waypointTolerance = 0.5f;

    [HideInInspector]
    public Transform[] waypoints;
    [HideInInspector]
    public int targetIndex;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;  // Assuming flying agents
        }

        if (waypointHandler != null)
        {
            waypoints = waypointHandler.waypoints;
            targetIndex = 0;
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        if (waypoints == null || waypoints.Length == 0)
            yield break;

        Vector3 currentWaypoint = waypoints[targetIndex].position;

        while (true)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) < waypointTolerance)
            {
                targetIndex++;
                if (targetIndex >= waypoints.Length)
                {
                    targetIndex = 0; // Loop back to the first waypoint
                }
                currentWaypoint = waypoints[targetIndex].position;
            }

            Vector3 direction = (currentWaypoint - transform.position).normalized;
            Vector3 velocity = direction * speed;
            rb.velocity = velocity;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);

            yield return null;
        }
    }
}