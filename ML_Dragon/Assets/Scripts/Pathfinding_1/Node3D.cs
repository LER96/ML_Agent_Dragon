using UnityEngine;

public class Node3D
{
    public bool isObstacle;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gridZ;

    public int gCost;
    public int hCost;
    public Node3D parent;

    public Node3D(bool isObstacle, Vector3 worldPosition, int gridX, int gridY, int gridZ)
    {
        this.isObstacle = isObstacle;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.gridZ = gridZ;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}