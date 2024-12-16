using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding{
    public abstract class PathFindingAlgorithm : MonoBehaviour
    {
        public abstract List<Node> FindPath(Vector3 startPosition, Vector3 endPosition);
    }
}
