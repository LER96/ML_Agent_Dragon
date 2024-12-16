using UnityEngine;
using System.Collections.Generic;

public class Pathfinding3D : MonoBehaviour
{
    public static Pathfinding3D Instance;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float nodeRadius = 0.5f;

    public Grid3D Grid { get; private set; }

    void Awake()
    {
        Instance = this;
        SetGridSizeBasedOnPoints();
    }

    void SetGridSizeBasedOnPoints()
    {
        if (startPoint != null && endPoint != null)
        {
            Vector3 size = endPoint.position - startPoint.position;
            Grid = new Grid3D(new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z)), nodeRadius, obstacleLayer);
        }
    }

    void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            FindPath(startPoint.position, endPoint.position);
        }
    }

    public List<Node3D> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node3D startNode = Grid.NodeFromWorldPoint(startPos);
        Node3D targetNode = Grid.NodeFromWorldPoint(targetPos);

        List<Node3D> openSet = new List<Node3D>();
        HashSet<Node3D> closedSet = new HashSet<Node3D>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node3D currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node3D neighbor in Grid.GetNeighbors(currentNode))
            {
                if (neighbor.isObstacle || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Node3D> RetracePath(Node3D startNode, Node3D endNode)
    {
        List<Node3D> path = new List<Node3D>();
        Node3D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    int GetDistance(Node3D nodeA, Node3D nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        return dstX + dstY + dstZ;
    }
}
