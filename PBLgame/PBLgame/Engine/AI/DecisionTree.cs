using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBLgame.Engine.AI
{
    public delegate void ActionDelegate();
    public delegate void DecisionDelegate();

    public class DecisionTree
    {
        private DTNode _dTreeStart;

        public DecisionTree()
        {
        }

        public DecisionTree(DTNode start)
        {
            _dTreeStart = start;
        }

        public DTNode DTreeStart
        {
            get { return _dTreeStart; }
            set { _dTreeStart = value; }
        }

        public void ExecuteTree()
        {
            if (DTreeStart != null)
            {
                _dTreeStart.ExecuteNode();
            }
        }
    }

    public abstract class DTNode
    {
        public virtual void ExecuteNode()
        {
            Console.WriteLine("Executed abstract ExectueNodeMethod");
        }
    }

    public class ActionNode : DTNode
    {
        public event ActionDelegate ActionEvent;

        public ActionNode()
        {
        }

        public override void ExecuteNode()
        {
            if (ActionEvent != null) ActionEvent();
        }
    }

    public class DecisionNode : DTNode
    {
        public event DecisionDelegate DecisionEvent;

        private DTNode _leftChild;
        private DTNode _rightChild;

        public DTNode LeftChild
        {
            get { return _leftChild; }
            set { _leftChild = value; }
        }

        public DTNode RightChild
        {
            get { return _rightChild; }
            set { _rightChild = value; }
        }

        public DecisionNode()
        {
            LeftChild = null;
            RightChild = null;
        }

        public DecisionNode(DecisionNode l, DecisionNode r)
        {
            LeftChild = l;
            RightChild = r;
        }

        public override void ExecuteNode()
        {
            if (LeftChild == null && RightChild == null)
            {
                Console.WriteLine("Decision Node Bug");
                return;
            }
            if (DecisionEvent != null) DecisionEvent();
        }
    }
}
