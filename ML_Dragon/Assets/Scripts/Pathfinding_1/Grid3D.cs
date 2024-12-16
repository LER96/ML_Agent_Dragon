using UnityEngine;
using System.Collections.Generic;

public class Grid3D
{
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public LayerMask obstacleLayer;
    public Node3D[,,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY, gridSizeZ;

    public Grid3D(Vector3 gridWorldSize, float nodeRadius, LayerMask obstacleLayer)
    {
        this.gridWorldSize = gridWorldSize;
        this.nodeRadius = nodeRadius;
        this.obstacleLayer = obstacleLayer;
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        if (gridSizeY == 0)
        {
            gridSizeY = 1;  // Ensure height is at least 1 to avoid zero-dimension array
        }

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node3D[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomLeft = new Vector3(-gridWorldSize.x / 2, -gridWorldSize.y / 2, -gridWorldSize.z / 2);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
                    bool isObstacle = Physics.CheckSphere(worldPoint, nodeRadius, obstacleLayer);
                    grid[x, y, z] = new Node3D(isObstacle, worldPoint, x, y, z);
                }
            }
        }

        Debug.Log($"Grid created with size: {gridSizeX}, {gridSizeY}, {gridSizeZ}");
    }

    public List<Node3D> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node3D startNode = NodeFromWorldPoint(startPos);
        Node3D targetNode = NodeFromWorldPoint(targetPos);

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

            foreach (Node3D neighbor in GetNeighbors(currentNode))
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

    public List<Node3D> GetNeighbors(Node3D node)
    {
        List<Node3D> neighbors = new List<Node3D>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbors.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbors;
    }

    public Node3D NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        // Ensure indices are within bounds
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);
        z = Mathf.Clamp(z, 0, gridSizeZ - 1);

        Debug.Log($"NodeFromWorldPoint: x={x}, y={y}, z={z}, gridSizeX={gridSizeX}, gridSizeY={gridSizeY}, gridSizeZ={gridSizeZ}");

        return grid[x, y, z];
    }

    public int GetDistance(Node3D nodeA, Node3D nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        return dstX + dstY + dstZ;
    }
}
