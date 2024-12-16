using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree{
    public class BehaviorTree : MonoBehaviour
    {
        private Node rootNode;

        void Start()
        {
            rootNode = SetupTree();
        }

        public void Update()
        {
            if (rootNode != null)
            {
                rootNode.Evaluate();
            }

            
        }

        protected virtual Node SetupTree()
        {
            // Override to set up the specific behavior tree
            return null;
        }
    }
}

