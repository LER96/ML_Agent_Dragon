using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding{
    public class AStar : PathFindingAlgorithm
    {
        public override List<Node> FindPath(Vector3 startPosition, Vector3 endPosition){
            Node startNode = Grid.Instance.NodeFromWorldPoint(startPosition);
            Node endNode =  Grid.Instance.NodeFromWorldPoint(endPosition);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while(openSet.Count > 0){
                Node currentNode = openSet[0];
                for( int i=0;i< openSet.Count;i++){
                    if(openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost){
                        currentNode = openSet[i];
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if( currentNode == endNode){
                    
                    return RetracePath(startNode,endNode);
                }

                foreach(Node neighbor in  Grid.Instance.GetNeighbors(currentNode)){
                    if( !neighbor.walkable || closedSet.Contains(neighbor)){
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode,neighbor);
                    if(newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)){
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor,endNode);
                        neighbor.parent = currentNode;

                        if(!openSet.Contains(neighbor)){
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private List<Node> RetracePath(Node startNode,Node endNode){
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while(currentNode != startNode){
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            //Debug.Log("Here");
            //grid.path = path;
            return path;
        }
        private int GetDistance(Node nodeA, Node nodeB) {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
            int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

            // Find the largest distance component
            int maxDist = Mathf.Max(distX, distY, distZ);
            // Find the middle distance component
            int midDist = distX + distY + distZ - Mathf.Min(distX, distY, distZ) - maxDist;

            return 14 * midDist + 10 * (maxDist - midDist);
        }

    }
}
