using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class Node 
    {
        public Node parent;
        public bool walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;
        public int gridZ;

        public int gCost;
        public int hCost;
        
        public int FCost{
            get => gCost + hCost;
        }
        public Node(bool walkable,Vector3 worldPosition, int gridX,int gridY, int gridZ){
            this.walkable = walkable;
            this.worldPosition = worldPosition;
            this.gridX = gridX;
            this.gridY = gridY;
            this.gridZ = gridZ;
        }
    }
}

