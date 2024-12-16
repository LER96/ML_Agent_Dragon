using UnityEngine;
using System.Collections.Generic;

public class GroundAgent : BaseAgent
{
    [SerializeField] private Pathfinding3D _pathfinding3D;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float nodeRadius = 0.5f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _collider;
    
    private Grid3D grid;
    private List<Node3D> path;
    private int pathIndex;

    protected override void Start()
    {
        _pathfinding3D = Pathfinding3D.Instance;
        
        grid = _pathfinding3D.Grid;

        FindPathToNextWaypoint();
    }

    void Update()
    {
        if (path != null && path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    protected override void MoveToNextWaypoint()
    {
        targetIndex = (targetIndex + 1) % waypoints.Length;
        FindPathToNextWaypoint();
    }

    private void FindPathToNextWaypoint()
    {
        Vector3 targetPosition = waypoints[targetIndex].position;
        path = grid.FindPath(transform.position, targetPosition);
        pathIndex = 0;

        if (path == null)
        {
            Debug.LogError("Path not found.");
        }
    }

    private void MoveAlongPath()
    {
        if (pathIndex >= path.Count)
        {
            MoveToNextWaypoint();
            return;
        }

        Node3D currentNode = path[pathIndex];
        Vector3 targetPosition = currentNode.worldPosition;

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Ensure the agent stays on the ground

        _rigidbody.MovePosition(transform.position + direction * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed);

        if (Vector3.Distance(transform.position, targetPosition) < nodeRadius)
        {
            pathIndex++;
        }
    }
}