using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PBLgame.Engine.AI;
using PBLgame.Engine.GameObjects;

namespace PBLgame.Engine.Components
{
    public class AIComponent : Component
    {
        private DecisionTree _myDTree;

        public DecisionTree MyDTree
        {
            get { return _myDTree; }
            set { _myDTree = value; }
        }

        public AIComponent(GameObject owner) : base(owner)
        {
            _myDTree = new DecisionTree();
            AISystem.AddAIObject(gameObject);
        }

        public AIComponent(GameObject owner, DTNode start): base(owner)
        {
            _myDTree = new DecisionTree(start);
            AISystem.AddAIObject(gameObject);
        }
    }
}
