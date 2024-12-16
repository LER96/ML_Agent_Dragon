using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace PathFinding{
    public class Grid : MonoBehaviour
    {
        public static Grid Instance { get; private set; }

        public LayerMask unwalkableMask;
        public Vector3 gridWorldSize;
        public float nodeRadius;
        public List<Node> path;

        public Color unwalkableColorCode,walkableColorCode,pathColorCode;
        public bool debug = false;
        
        private Node[,,] grid;
        private float nodeDiameter;
        private int gridSizeX,gridSizeY,gridSizeZ; 

        void Awake(){
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);  // Make the singleton persistent across scenes
            }
            else if (Instance != this)
            {
                Destroy(gameObject);  // Destroy duplicate instance
            }
        }
        void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y/ nodeDiameter);
            gridSizeZ = Mathf.RoundToInt(gridWorldSize.z/ nodeDiameter);

            CreateGrid();
        }

        private void CreateGrid(){
            grid = new Node[gridSizeX,gridSizeY,gridSizeZ];
            Debug.Log(grid);
            Vector3 worldOrigin = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward* gridWorldSize.z/2;

            for (int x = 0;x < gridSizeX; x++){
                for( int y = 0; y < gridSizeY; y++){
                    for( int z =0; z < gridSizeZ; z++){
                        Vector3 worldPoint = worldOrigin + Vector3.right * ( x * nodeDiameter + nodeRadius) +  Vector3.up * ( y * nodeDiameter + nodeRadius) +
                        Vector3.forward * ( z * nodeDiameter + nodeRadius);
                        bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
                        grid[x,y,z] = new Node(walkable,worldPoint,x,y,z);
                    }
                }
            }
        }
        
        public Node NodeFromWorldPoint(Vector3 worldPoint){
            float percentX = (worldPoint.x + gridWorldSize.x/2) / gridWorldSize.x;
            float percentY = (worldPoint.y) / gridWorldSize.y;
            float percentZ = (worldPoint.z + gridWorldSize.z/2) / gridWorldSize.z;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
            percentZ = Mathf.Clamp01(percentZ);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

            return grid[x,y,z];
        }

        public List<Node> GetNeighbors(Node node){
            List<Node> neighbors = new List<Node>();

            for( int x =-1;x<=1;x++){
                for( int y =-1;y<=1;y++){
                    for( int z =-1;z<=1;z++){
                        if( x ==0 && y ==0 && z ==0){
                            continue;
                        }

                        int checkX = node.gridX + x;
                        int checkY = node.gridY + y;
                        int checkZ = node.gridZ + z;

                        if(checkX >=0 && checkX < gridSizeX && checkY >=0 && checkY < gridSizeY && checkZ >=0 && checkZ < gridSizeZ){
                            neighbors.Add(grid[checkX,checkY,checkZ]);
                        }
                    }
                }
            }
            return neighbors;
        }
        void OnDrawGizmos(){
            if(!debug){
                return;
            }
            Gizmos.DrawWireCube(transform.position + Vector3.up * gridWorldSize.y/2, gridWorldSize);

            if(grid != null){
                foreach(Node node in grid){
                    Gizmos.color = node.walkable? walkableColorCode : unwalkableColorCode;
                    if(path!=null && path.Contains(node)){
                        Gizmos.color = pathColorCode; 
                    }
                    Gizmos.DrawSphere(node.worldPosition, nodeDiameter/2f);
                }
            }
        }
    }
}
